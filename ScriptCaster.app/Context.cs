

namespace ScriptCaster.app
{
    
    public class Context
    {

        #region Singleton
        private static Context instance = null;

        private Context()
        {
            Initiated = false;
        }

        public static Context Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Context();
                }
                return instance;
            }
        }
        #endregion


        public string TemplateName { get; private set; }
        public string TemplatePath { get; private set; }
        public string TemplateVariablePath { get; private set; }
        public string LocalPath { get; private set; }
        public string GlobalVariablePath { get; private set; }
        public int Recursivity {get; private set; }
        public bool Initiated {get; private set; }

        //TODO: the template path shall also be a config in ~/.config/ScriptCatser/config
        //TODO: Recursivity shall also be a config
        public void InitContext(string? templateName, string templatesFolderPath, int recursivity) {
            if (templateName == null) return;

            TemplateName = templateName;

            if(!Directory.Exists(templatesFolderPath)) {
                Logger.LogError($"{templatesFolderPath} does not exist");
                Logger.LogError($"   this is the folder that should contain all the templates");
                Logger.LogError($"   please create the folder then try create a new template with :");
                Logger.Log($"      sc {templateName} -c", ConsoleColor.Cyan);
                return;
            }

            TemplatePath = $"{templatesFolderPath}/{templateName}";
            
            if(!Directory.Exists(TemplatePath)) {
                Logger.LogError($"{TemplatePath} does not exist");
                Logger.LogError($"   this is the folder that contain the template and local information about it");
                Logger.LogError($"   You may want to create a new template, here is the command : ");
                Logger.Log($"      sc {templateName} -c", ConsoleColor.Cyan);
                return;
            }

            
            TemplateVariablePath = $"{TemplatePath}/.variables.json";

            LocalPath = ScriptCaster.Services.Process.GetPwd();
            GlobalVariablePath = $"{templatesFolderPath}/.variables.json";
            Recursivity = recursivity;

            Initiated = true;
        }

        public void ListTemplates(string path) {
            var templateList = ScriptCaster.Services.Process.GetAllFilesAndFolders(path).Where(n => Directory.Exists(n));

            Console.WriteLine($"I found {templateList.Count()} template{(templateList.Count() > 1 ? "s" : "")} :");

            foreach(var template in templateList) {
                var segmentedTemplatePath = template.Split("/");
                var templateName = segmentedTemplatePath[segmentedTemplatePath.Count() - 1];
                Console.WriteLine($"   - {templateName}");
            }

            Console.WriteLine();
            Console.WriteLine("   Do not forget that only folders will be considered as template.");
            Console.WriteLine("If you whant just a file, touch it in a folder");
        }
    }
}