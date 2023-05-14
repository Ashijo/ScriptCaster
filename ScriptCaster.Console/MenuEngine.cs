using ScriptCaster.Console.CmdControllers;
using ScriptCaster.Console.Enums;
using ScriptCaster.Core.Enums;

namespace ScriptCaster.Console;

public static class MenuEngine
{
	public static void StartMenu()
	{
		var run = true;
		while (run)
		{
			var menuResult = MenuCmdController.StartMenu();

			switch (menuResult)
			{
				case EStartMenuChoices.ListTemplates:
					Worker.ListTemplates();
					break;
				case EStartMenuChoices.SelectTemplate:
					Worker.SelectTemplate();
					break;
				case EStartMenuChoices.CreateNewTemplate:
					Worker.CreateNewTemplate();
					break;
				case EStartMenuChoices.UpdateTemplateVariable:
					Worker.UpdateVariables(EVariableFile.Template);
					break;
				case EStartMenuChoices.UpdateGlobalVariable:
					Worker.UpdateVariables(EVariableFile.Global);
					break;
				case EStartMenuChoices.Cast:
					Worker.Cast();
					break;
				case EStartMenuChoices.CastForce:
					Worker.Cast(true);
					break;
				case EStartMenuChoices.PreviewCast:
					Worker.PreviewCast();
					break;
				case EStartMenuChoices.Quit:
					run = false;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
