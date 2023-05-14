using ScriptCaster.Console.CmdControllers;
using ScriptCaster.Core;
using ScriptCaster.Core.Enums;
using ScriptCaster.Core.Services;

namespace ScriptCaster.Console;

public static class Worker
{
	public static void ListTemplates()
	{
		var (success, templateList) = Context.ListTemplates();

		//TODO: Manage exception
		if (!success)
		{
			throw new Exception("MMmmmh");
		}

		Logger.Log($"I found {templateList.Count()} template{(templateList.Count() > 1 ? "s" : "")} :");

		foreach (var template in templateList)
		{
			Logger.Log($"   - {template}");
		}
	}

	public static void SelectTemplate()
	{
		var template = MenuCmdController.ChooseATemplateFromList();

		if (string.IsNullOrWhiteSpace(template))
		{
			return;
		}

		Context.SelectTemplate(template);
	}

	public static void CreateNewTemplate()
	{
		var templateName = Context.TemplateName;

		while (string.IsNullOrWhiteSpace(templateName))
		{
			System.Console.WriteLine("No template name defined, please enter a name :");
			templateName = System.Console.ReadLine();
		}

		Create.CreateNewTemplate(templateName);

		Logger.LogSuccess($"New template \"{templateName}\" created");
		Logger.Log("Do you want to add variables to your template ? y/n (default=y)");
		var answer = System.Console.ReadLine()?.Trim().ToUpper();

		var templateVariableUpdate = string.IsNullOrEmpty(answer) || answer is "Y" or "YES";

		if (templateVariableUpdate)
		{
			UpdateVariables(EVariableFile.Template);
		}
	}

	public static void UpdateVariables(EVariableFile eVariableFile)
	{
		SetVariableCmdController.LaunchVariableSetting(eVariableFile);
	}

	public static void Cast()
	{
		Core.Services.Cast.LaunchOldCast();
	}

	public static void PreviewCast()
	{
		throw new NotImplementedException();
	}
}
