using System.Diagnostics;
using ScriptCaster.Core.Callbacks;
using ScriptCaster.Core.Helpers;

namespace ScriptCaster.Core;

public static class Context
{
	public static string? TemplatesCollectionPath { get; private set; }
	public static string? TemplateName { get; private set; }
	public static string? LocalPath { get; private set; }
	public static string TemplatePath => $"{TemplatesCollectionPath}/{TemplateName}";
	public static string TemplateVariablePath => $"{TemplatePath}/.variables.json";
	public static string GlobalVariablePath => $"{TemplatesCollectionPath}/.variables.json";
	public static int? RecursionLevel { get; private set; }
	public static bool Initiated { get; private set; }

	//TODO: the default template path shall also be a config in ~/.config/ScriptCaster/config
	//TODO: Default recursionLevel shall also be a config
	public static RInitContextCallback InitContext(string? templateName, string templatesCollectionPath,
		int recursionLevel = 3)
	{
		TemplatesCollectionPath = templatesCollectionPath;

		if (!string.IsNullOrWhiteSpace(templateName))
		{
			TemplateName = templateName;
		}

		if (!Directory.Exists(templatesCollectionPath))
		{
			return new RInitContextCallback(false);
		}

		LocalPath = Directory.GetCurrentDirectory();
		RecursionLevel = recursionLevel;

		Initiated = true;

		return new RInitContextCallback(true);
	}

	public static RListTemplateCallback ListTemplates()
	{
		Debug.Assert(TemplatesCollectionPath != null,
			nameof(TemplatesCollectionPath) + " should never be null... But is :<");
		var templateList = DirectoryHelper.GetDirectoriesName(TemplatesCollectionPath);

		return new RListTemplateCallback(true, templateList);
	}

	public static RSelectTemplateCallback SelectTemplate(string templateToSelect)
	{
		//TODO: Better validation of template name 

		if (!Directory.Exists($"{TemplatesCollectionPath}/{templateToSelect}"))
		{
			return new RSelectTemplateCallback(ESelectTemplateCallbackStatus.NotFound);
		}

		TemplateName = templateToSelect;

		return new RSelectTemplateCallback(ESelectTemplateCallbackStatus.Success);
	}
}
