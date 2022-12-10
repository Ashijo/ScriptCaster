using ScriptCaster.Services;
using Spectre.Console;

namespace ScriptCaster.App.CmdController;

public static class MenuCmdController
{
    public static void StartMenu()
    {
        var command = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Welcome to ScriptCaster. We are working on the folder {Context.TemplatesCollectionPath} [cyan]What do you want to do ?[/]")
                .PageSize(8)
                .AddChoices(
                    "[cyan]List my templates[/]", 
                    "[cyan]Create a new template folder[/]",
                    "[cyan]Update the variables of a template[/]", 
                    "[cyan]Update the global variables[/]",
                    "[cyan]Cast a script[/]", 
                    "[cyan]Preview a cast[/]",
                    "[cyan]Quit [/]"));

        var parsedCommand = command.Remove(0, 6).Split(' ');
        AnsiConsole.WriteLine(parsedCommand[0]);
    }
}