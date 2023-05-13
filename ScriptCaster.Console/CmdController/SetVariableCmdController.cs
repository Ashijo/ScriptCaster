using ScriptCaster.Core;
using ScriptCaster.Core.Enums;
using ScriptCaster.Core.Services;
using Spectre.Console;

namespace ScriptCaster.Console.CmdController;

/*
* Set variable class shall allow me to edit the .variables.json files
* Depending on the option, either editing the global variables or the variable of the template
*/
public static class SetVariableCmdController
{
	private static VariableFile _currentTarget;

	private static readonly Dictionary<VariableFile, SetVariable> SetVariableServices = new()
	{
		{ VariableFile.Global, new SetVariable(Context.GlobalVariablePath) },
		{ VariableFile.Template, new SetVariable(Context.TemplateVariablePath) }
	};

	private static string CurrentTargetStr =>
		_currentTarget == VariableFile.Global ? "global variable file" : "template variable file";

	public static void LaunchVariableSetting(VariableFile variableFile)
	{
		_currentTarget = variableFile;
		ReadInput(OptionsDisplay());
	}


	private static string[] OptionsDisplay()
	{
		var command = AnsiConsole.Prompt(
			new SelectionPrompt<string>()
				.Title($"We are working with the [green]{CurrentTargetStr}[/]. [cyan]What do you want to do ?[/]")
				.PageSize(8)
				.AddChoices("[cyan]list variables[/]", "[cyan]add a new variable to the buffer[/]",
					"[cyan]remove a variable from the buffer[/]", "[cyan]edit a variable in the buffer[/]",
					"[cyan]write the changes in the buffer[/]", "[cyan]change the variable target[/]",
					"[cyan]quit [/]"));

		var parsedCommand = command.Remove(0, 6).Split(' ');
		AnsiConsole.WriteLine(parsedCommand[0]);

		return parsedCommand;
	}

	private static void ReadInput(IReadOnlyList<string> cmd)
	{
		switch (cmd[0])
		{
			case "list":
				List();
				break;
			case "add":
				var addKey = AnsiConsole.Ask<string>("What will be the name (or key) of the new variable ?");
				var addValue = AnsiConsole.Prompt(
					new TextPrompt<string>("[grey][[Optional]][/] And what is the value ?")
						.AllowEmpty());

				Add(addKey, addValue);

				break;
			case "remove":
				var removeKey = AnsiConsole.Ask<string>("What is the name (or key) of the variable to remove?");
				Remove(removeKey);
				break;
			case "edit":
				var editKey = AnsiConsole.Ask<string>("What is the name (or key) of the variable to edit?");
				var editValue = AnsiConsole.Prompt(
					new TextPrompt<string>("[grey][[Optional]][/] And what is the new value ?")
						.AllowEmpty());
				Edit(editKey, editValue);
				break;
			case "write":
				Write();
				break;
			case "change":
				SwitchTarget();
				break;
			case "quit":
				return;
			default:
				System.Console.WriteLine("input not understood");
				break;
		}

		ReadInput(OptionsDisplay());
	}

	private static void List()
	{
		var table = new Table();

		table.AddColumn(new TableColumn("Actual variable").Centered());
		table.AddColumn(new TableColumn("Buffer variable").Centered());

		var actualVariables = SetVariableServices[_currentTarget].GetActualVariables();
		var bufferVariables = SetVariableServices[_currentTarget].GetBuffer();

		table.AddRow(ShowVariablesFromDictionary(actualVariables), ShowVariablesFromDictionary(bufferVariables));
		AnsiConsole.Write(table);
	}


	private static Table ShowVariablesFromDictionary(Dictionary<string, string> actualVariables)
	{
		var table = new Table();

		table.AddColumn(new TableColumn("Key").Centered());
		table.AddColumn(new TableColumn("Value").Centered());

		foreach (var keyValuePair in actualVariables) table.AddRow(keyValuePair.Key, keyValuePair.Value);

		return table;
	}

	private static void Add(string newVariableName, string newVariableValue)
	{
		var success = SetVariableServices[_currentTarget].AddVariableToBuffer(newVariableName, newVariableValue);

		if (!success) AnsiConsole.Confirm("The variable already exist in the buffer.. Operation cancelled.");
	}

	private static void Remove(string key)
	{
		var success = SetVariableServices[_currentTarget].RemoveVariableToBuffer(key);
		if (!success) AnsiConsole.Confirm("The variable key didn't exist in the buffer.");
	}

	private static void Edit(string editKey, string editValue)
	{
		var success = SetVariableServices[_currentTarget].EditVariableToBuffer(editKey, editValue);
		if (!success) AnsiConsole.Confirm("Variable not found :/");
	}

	private static void Write()
	{
		var result = SetVariableServices[_currentTarget].WriteBuffer();
		var table = ShowVariablesFromDictionary(result);
		AnsiConsole.Write(table);
	}

	private static void SwitchTarget()
	{
		_currentTarget = _currentTarget == VariableFile.Global ? VariableFile.Template : VariableFile.Global;
	}
}
