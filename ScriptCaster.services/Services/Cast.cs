using System.Diagnostics;
using Newtonsoft.Json;
using ScriptCaster.Services.Extensions;
using ScriptCaster.Services.Helpers;

namespace ScriptCaster.Services.Services
{
    /*
    Cast shall read the template, the local variable file and the global variable file
    variable files are json dictionarie <string,string>, every occurence of the key shall be replace by the value
    TODO: default key to add:
        %NEWGUID%
        %DATE% and co

    TODO: find a way to format dates
    
    
    TODO: Format with pipes (eg : %Foo | Capitalize%)

    Nice to have :
        Logical variable : use reflexion to execute a C# function define in other variable script
        Bash variable : set variable as the result of a bash command
    */
    public static class Cast
    {
        public static void LaunchCast() {
            if (!Context.Instance.Initiated) return;

            Debug.Assert(Context.Instance.GlobalVariablePath != null, "GlobalVariablePath is null in Cast.LaunchCast");
            var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                File.ReadAllText(Context.Instance.GlobalVariablePath));

            Debug.Assert(Context.Instance.TemplateVariablePath != null, "TemplateVariablePath is null in Cast.LaunchCast");
            var templateVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                File.ReadAllText(Context.Instance.TemplateVariablePath));

            //TODO Refactoring: Move this validation somewhere else 
            if(variables == null || templateVariables == null) {
                var fileNotExistsInGlobal = variables == null;
                var fileNotExistsInTemplate = templateVariables == null;

                var fileNotExistsInGlobalMsg = $"global folder ({Context.Instance.GlobalVariablePath}) ";
                var fileNotExistsInTemplateMsg = $"template folder ({Context.Instance.TemplateVariablePath}) ";

                var fileNotExistsIn = fileNotExistsInGlobal ? 
                    ( fileNotExistsInTemplate ? 
                        $"{fileNotExistsInGlobalMsg} and {fileNotExistsInTemplateMsg}" 
                        : fileNotExistsInGlobalMsg
                    ) : fileNotExistsInTemplateMsg;

                Logger.LogError($".variables.json does not exists in {fileNotExistsIn}.");
                Logger.LogWarning("You can create it running :");
                Logger.Log($"   sc {(fileNotExistsInTemplate ? "<TemplateName>" : "")} -{(fileNotExistsInGlobal ? "g" : "")}{(fileNotExistsInTemplate ? "t" : "")}", ConsoleColor.Cyan);
                
                return;
            }

            variables.AddRangeOverride(templateVariables);
            var templateFolders = GetAllFolders();
            
            CreateFolders(variables, templateFolders);
            var filesInTemplate = GetAllFiles(templateFolders);

            foreach(var tFilePath in filesInTemplate) {
                
                Debug.Assert(Context.Instance.TemplatePath != null, "TemplatePath is null in Cast.LaunchCast");
                var rFilePath = tFilePath
                    .Replace(Context.Instance.TemplatePath, Context.Instance.LocalPath)
                    .Replace("\n", "");

                if(File.Exists(rFilePath)) {
                    Logger.LogWarning($"{rFilePath} already exist. Ignored. Soon(~ish) force option will be add.");
                    continue;
                }

                var fileContent = File.ReadAllText(tFilePath);

                
                for(var i = 0; i < Context.Instance.Recursivity; i++) {
                    foreach(var kv in variables) {
                        rFilePath = rFilePath.Replace(kv.Key, kv.Value);
                        fileContent = fileContent.Replace(kv.Key, kv.Value);
                    }
                }

                using var stream = File.CreateText(rFilePath);
                stream.Write(fileContent);
                stream.Close();
            }

        } 

        private static void CreateFolders(Dictionary<string, string> variables, string[] templatesFolders) {
            

            foreach(var tFolder in templatesFolders) {
                Debug.Assert(Context.Instance.TemplatePath != null, "TemplatePath is null in Cast.CreateFolders");
                var rFolder = tFolder
                    .Replace(Context.Instance.TemplatePath, Context.Instance.LocalPath)
                    .Replace("\n", "");

                for(int i = 0; i < Context.Instance.Recursivity; i++) {
                    foreach(var kv in variables) {
                        rFolder = rFolder.Replace(kv.Key, kv.Value);
                    }
                }

                Directory.CreateDirectory(rFolder);
            }
        }

        private static string[] GetAllFolders()
        {
            Debug.Assert(Context.Instance.TemplatePath != null, "TemplatePath is null in Cast.GetAllFolders");
            return GetFoldersFromParent(Context.Instance.TemplatePath);
        }

        private static string[] GetFoldersFromParent(string directory) {
            var list = new List<string>();
            var foldersInDir = Directory.GetDirectories(directory);

            foreach(var folder in foldersInDir) {
                list.Add(folder);
                list.AddRange(GetFoldersFromParent(folder));
            }

            return list.ToArray();
        }

        //There is a way to merge GetAllFolder and GetAllFile
        //It would be a lot better optimization
        //But also harder to read
        private static string[] GetAllFiles(string[] templateFolders)
        {
            var files = new List<string>();
            foreach (var folderPath in templateFolders)
            {
                files.AddRange(DirectoryHelper.GetAllFilesWithoutConfigs(folderPath));
            }
            return files.ToArray();
        }

    }
}