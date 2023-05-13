using System.Text;
using McMaster.Extensions.CommandLineUtils;
using ScriptCaster.Console;
using ScriptCaster.Console.CmdControllers;
using ScriptCaster.Core;
using ScriptCaster.Core.Enums;
using ScriptCaster.Core.Services;

[Command(Name = "ScriptCaster", Description = "Grab a folder, replace variables and paste it wherever you are")]
[HelpOption("-?|-h|--help")]
internal class Program
{
	[Argument(0, Name = "TemplateName", Description = "The name of the template we have to work with")]
	private string? TemplateName { get; } = null;

	[Option("-t|--template", Description = "Update the variables files of the template")]
	private bool TemplateVariableUpdate { get; set; }

	[Option("-g|--global", Description = "Update the global variables files")]
	private bool GlobalVariableUpdate { get; } = false;

	[Option("-f|--force",
		Description =
			"Force the replacement of already existing files with new ones (I hope you know what you are doing)")]
	private bool Force { get; } = false;

	[Option(Description = "List of all templates")]
	private bool List { get; } = false;

	//IDEA: Create template from other template ?    
	[Option(Description = "Create a new empty template")]
	private bool Create { get; } = false;

	//TODO: get the default recursive from .config
	[Option(Description =
		"The number of time parsing will be done on every text, bigger it is the more depth you can have in variables references")]
	private int RecursionLevel { get; } = 3;

	//TODO: get the default path from .config
	[Option("-p|--path", Description = "Path of the folder containing the template")]
	private string TemplatesCollectionPath { get; } =
		$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Templates";

	private static void Main(string[] args)
	{
		CommandLineApplication.Execute<Program>(args);
	}

	private void OnExecute()
	{
		var result = Context.InitContext(TemplateName, TemplatesCollectionPath, RecursionLevel);

		if (!result.TemplateCollectionPathExist)
		{
			Logger.LogError($"{TemplatesCollectionPath} does not exist");
			Logger.LogError("   this is the folder that should contain all the templates");
			Logger.LogError("   please create this folder ");
			return;
		}

		if (List)
		{
			Worker.ListTemplates();
			return;
		}

		if (Create)
		{
			Worker.CreateNewTemplate();
			return;
		}

		if (TemplateVariableUpdate || GlobalVariableUpdate)
		{
			switch (TemplateVariableUpdate)
			{
				case true when string.IsNullOrEmpty(TemplateName):
					Logger.LogError("Need the name of the template to update his variables");
					return;
				case true:
					Worker.UpdateVariables(VariableFile.Template);
					break;
			}

			if (GlobalVariableUpdate)
			{
				Worker.UpdateVariables(VariableFile.Global);
			}

			return;
		}

		if (string.IsNullOrEmpty(TemplateName))
		{
			MenuEngine.StartMenu();
			return;
		}

		var castCallback = Cast.LaunchCast(Force);

		if (castCallback.Success)
		{
			Logger.LogSuccess("Cast finished");
			return;
		}

		ErrorCmdController.PromptCastCallbackError(castCallback);
	}
}
