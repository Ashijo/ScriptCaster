namespace ScriptCaster.Core.Services;

//Create a new template folder with .variable.json and launch variable setup
public static class Create
{
	public static void CreateNewTemplate(string templateName)
	{
		if (string.IsNullOrWhiteSpace(templateName))
		{
			throw new Exception("New template name invalid");
		}


		var directoryInfo = new DirectoryInfo($"{Context.TemplatePath}/{templateName}");
		directoryInfo.Create();
		
		Context.SelectTemplate(templateName);

		if (string.IsNullOrWhiteSpace(Context.TemplateVariablePath))
		{
			directoryInfo.Delete();
			return;
		}

		File.Create(Context.TemplateVariablePath);
	}
}
