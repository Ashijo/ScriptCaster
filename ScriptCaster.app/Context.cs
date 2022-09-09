

namespace ScriptCaster.app
{
    
    public class Context
    {

        #region Singleton
        private static Context instance = null;

        private Context()
        {
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

        //TODO: the template path shall be a config in ~/.config/ScriptCatser/config
        //TODO: Recursivity shall be a config or an option
        public void InitContext(string templateName) {
            TemplateName = templateName;
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            TemplatePath = $"{home}/Templates/{templateName}";
            TemplateVariablePath = $"{TemplatePath}/.variables.json";

            LocalPath = ScriptCaster.Services.Process.GetPwd();

            GlobalVariablePath = $"{home}/Templates/.variables.json";

            Recursivity = 3;
        }
    }
}