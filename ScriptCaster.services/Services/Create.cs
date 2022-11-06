namespace ScriptCaster.Services.Services
{
    //Create a new template folder with .variable.json and launch variable setup
    public static class Create
    {
        public static void CreateNewTemplate()
        {
            if (Context.Instance.TemplatePath == null)
            {
                Logger.LogError("New template path no found");
                return;
            }

            var directoryInfo = new DirectoryInfo(Context.Instance.TemplatePath);
            directoryInfo.Create();
        
            if (Context.Instance.TemplateVariablePath == null)
            {
                directoryInfo.Delete();
                return;
            }

            File.Create(Context.Instance.TemplateVariablePath);
        }
    }
}