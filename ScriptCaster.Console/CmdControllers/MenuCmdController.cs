using System.Text;
using ScriptCaster.Console.Enums;
using ScriptCaster.Console.Utils;
using ScriptCaster.Core;
using Spectre.Console;

namespace ScriptCaster.Console.CmdControllers;

public static class MenuCmdController
{
	public static EStartMenuChoices StartMenu()
	{
		var title = new StringBuilder();
		title.AppendLine("Welcome to ScriptCaster.");
		title.AppendLine($"We are working on the folder {Context.TemplatesCollectionPath} ");

		var templateSelected = !string.IsNullOrWhiteSpace(Context.TemplateName);

		if (templateSelected)
		{
			title.AppendLine($"   > Template selected : {Context.TemplateName}");
		}
		else
		{
			title.AppendLine("   > No Template selected");
		}

		title.AppendLine("[cyan]What do you want to do ?[/]");

		var choicesStrList = StartMenuChoicesEnumUtils.GetStartMenuChoicesStrings();

		var command = AnsiConsole.Prompt(
			new SelectionPrompt<string>()
				.Title(title.ToString())
				.PageSize(choicesStrList.Length)
				.AddChoices(choicesStrList));

		return StartMenuChoicesEnumUtils.GetMenuChoiceEnumFromString(command);
	}

	public static string? ChooseATemplateFromList()
	{
		var result = Context.ListTemplates();

		var list = result.Templates;

		//TODO: manage it better
		if (!list.Any())
		{
			Logger.LogError("Templates folder empty");
			return null;
		}

		if (list.Length == 1)
		{
			Logger.Log("Only one template found. Auto select " + list.First());
			return list.First();
		}

		//for a weird reason Selection Prompt cannot let you choose between 2 choices
		var trick = list.Length == 2
			? list.Append("I hate you spectre [red]This choice is a bug[/]").ToArray()
			: list;
		
		return AnsiConsole.Prompt(
			new SelectionPrompt<string>()
				.Title("Choose a template :")
				.PageSize(trick.Length)
				.AddChoices(trick));
	}
}
