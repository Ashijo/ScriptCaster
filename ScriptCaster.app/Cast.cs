
using Newtonsoft.Json;
using ScriptCaster.Services;

namespace ScriptCaster.app
{
    /*
    Cast shall read the template, the local variable file and the global variable file
    variable files are json dictionarie <string,string>, every occurence of the key shall be replace by the value
    TODO: default key to add:
        %NEWGUID%
        %DATE% and co

    TODO: find a way to format dates

    Nice to have :
        Logical variable : use reflexion to execute a C# function define in other variable script
        Bash variable : set variable as the result of a bash command
    */
    public static class Cast
    {
        static int depth = 0;

        public static void LaunchCast() {
            if (!Context.Instance.Initiated) return;

            var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                File.ReadAllText(Context.Instance.GlobalVariablePath));

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

                if(variables == null && templateVariables == null) {
                }

                Logger.LogError($".variables.json does not exists {fileNotExistsIn}.");
                Logger.LogWarning("You can create it running :");
                Logger.Log($"   sc {(fileNotExistsInTemplate ? "<TemplateName>" : "")} -{(fileNotExistsInGlobal ? "g" : "")}{(fileNotExistsInTemplate ? "t" : "")}", ConsoleColor.Cyan);
                return;
            }

            variables?.AddRangeOverride(templateVariables);

            CreateFolders(variables);
            var filesInTemplate = GetAllFiles();

            foreach(var tFilePath in filesInTemplate) {

                var rFilePath = tFilePath
                    .Replace(Context.Instance.TemplatePath, Context.Instance.LocalPath)
                    .Replace("\n", "");

                if(File.Exists(rFilePath)) {
                    Logger.LogWarning($"{rFilePath} already exist. Ignored. Soon(~ish) force option will be add.");
                    continue;
                }

                var fileContent = File.ReadAllText(tFilePath);

                
                for(int i = 0; i < Context.Instance.Recursivity; i++) {
                    foreach(var kv in variables) {
                        rFilePath = rFilePath.Replace(kv.Key, kv.Value);
                        fileContent = fileContent.Replace(kv.Key, kv.Value);
                    }
                }

                using (var stream = File.CreateText(rFilePath)){
                    stream.Write(fileContent);
                    stream.Close();
                }
                
            }

        } 

        private static void CreateFolders(Dictionary<string, string> variables) {
            var foldersInTemplate = GetAllFolders();

            foreach(var tFolder in foldersInTemplate) {
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

        private static string[] GetAllFolders() {
            return GetFoldersFromParent(Context.Instance.TemplatePath);
        }

        private static string[] GetFoldersFromParent(string directory) {
            var list = new List<string>();
            var foldersInDir = Process.GetAllFilesAndFolders(directory);

            foreach(var folderName in foldersInDir) {
                if(Directory.Exists($"{directory}/{folderName}")) {
                    list.Add($"{directory}/{folderName}");
                    list.AddRange(GetFoldersFromParent($"{directory}/{folderName}"));
                } 
            }

            return list.ToArray();
        }

        //There is a way to merge GetAllFolder and GetAllFile
        //It would be a lot better optimization wize
        //But also harder to read
        private static string[] GetAllFiles() {
            return GetFilesFromParent(Context.Instance.TemplatePath);
        }

        private static string[] GetFilesFromParent(string directory) {
            var list = new List<string>();
            var filesInDir = Process.GetAllFilesAndFolders(directory);

            foreach(var fileName in filesInDir) {
                if(!Directory.Exists($"{directory}/{fileName}")) {
                    list.Add($"{directory}/{fileName}");
                } else {
                    list.AddRange(GetFilesFromParent($"{directory}/{fileName}"));
                } 
            }

            return list.ToArray();
        }

    }
}