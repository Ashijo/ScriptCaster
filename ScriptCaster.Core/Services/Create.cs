namespace ScriptCaster.Core.Services;

//Create a new template folder with .variable.json and launch variable setup
public static class Create
{
	public static void CreateNewTemplate()
	{
		if (Context.TemplatePath == null)
		{
			Logger.LogError("New template path not found");
			return;
		}

		var directoryInfo = new DirectoryInfo(Context.TemplatePath);
		directoryInfo.Create();

		if (Context.TemplateVariablePath == null)
		{
			directoryInfo.Delete();
			return;
		}

		File.Create(Context.TemplateVariablePath);
	}

	public static void CreateTemplateFolder()
	{
		if (Context.TemplatesCollectionPath == null)
		{
			return;
		}

		var directoryInfo = new DirectoryInfo(Context.TemplatesCollectionPath);
	}
}
