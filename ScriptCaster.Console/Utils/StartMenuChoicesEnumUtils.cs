using ScriptCaster.Console.Enums;

namespace ScriptCaster.Console.Utils;

public static class StartMenuChoicesEnumUtils
{
	private static readonly (EStartMenuChoices Key, string Value)[] _startMenuChoices =
		new[]
		{
			(EStartMenuChoices.ListTemplates, "[cyan]List my templates[/]"),
			(EStartMenuChoices.SelectTemplate, "[cyan]Select a template[/]"),
			(EStartMenuChoices.CreateNewTemplate, "[cyan]Create a new template folder[/]"),
			(EStartMenuChoices.UpdateTemplateVariable, "[cyan]Update the variables of a template[/]"),
			(EStartMenuChoices.UpdateGlobalVariable, "[cyan]Update the global variables[/]"),
			(EStartMenuChoices.Cast, "[cyan]Cast[/]"),
			(EStartMenuChoices.CastForce, "[cyan]Cast force (will override existing files, use with caution)[/]"),
			(EStartMenuChoices.PreviewCast, "[cyan]Preview a cast[/] [red](NOT IMPLEMENTED)[/]"),
			(EStartMenuChoices.Quit, "[cyan]Quit [/]")
		};

	public static string[] GetStartMenuChoicesStrings()
	{
		return _startMenuChoices.Select(c => c.Value).ToArray();
	}

	public static EStartMenuChoices[] GetStartMenuChoicesEnums()
	{
		return _startMenuChoices.Select(c => c.Key).ToArray();
	}

	public static EStartMenuChoices GetMenuChoiceEnumFromString(string value)
	{
		var result = _startMenuChoices.First(c => c.Value == value).Key;
		if (!Enum.IsDefined(result))
		{
			throw new Exception($"Internal error: MenuChoice does not exist in {nameof(StartMenuChoicesEnumUtils)}");
		}

		return result;
	}

	public static string GetMenuChoiceStringFromEnum(EStartMenuChoices key)
	{
		if (!Enum.IsDefined(key))
		{
			throw new Exception($"Internal error: MenuChoice does not exist in {nameof(StartMenuChoicesEnumUtils)}");
		}

		return _startMenuChoices.First(c => c.Key == key).Value;
	}
}
