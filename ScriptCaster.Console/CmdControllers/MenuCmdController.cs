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
		var title = new StringBuilder("Welcome to ScriptCaster.");
		title.Append($"We are working on the folder {Context.TemplatesCollectionPath} ");
		title.Append("[cyan]What do you want to do ?[/]");

		var choicesStrList = StartMenuChoicesEnumUtils.GetStartMenuChoicesStrings();

		var command = AnsiConsole.Prompt(
			new SelectionPrompt<string>()
				.Title(title.ToString())
				.PageSize(choicesStrList.Length)
				.AddChoices(choicesStrList));

		return StartMenuChoicesEnumUtils.GetMenuChoiceEnumFromString(command);
	}
}
