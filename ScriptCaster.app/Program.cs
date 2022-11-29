using McMaster.Extensions.CommandLineUtils;
using ScriptCaster.App.CmdController;
using ScriptCaster.Services;
using ScriptCaster.Services.Enums;
using ScriptCaster.Services.Services;

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
    private int Recursivity { get; } = 3;

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
        Context.InitContext(TemplateName, TemplatesCollectionPath, Recursivity, Force, Create);
        
        if (List)
        {
            Context.ListTemplates();
            return;
        }

        if (Create)
        {
            ScriptCaster.Services.Services.Create.CreateNewTemplate();
            Logger.LogSuccess($"New template \"{Context.TemplateName}\" created");
            Logger.Log("Do you want to add variables to your template ? y/n (default=y)");
            var answer = Console.ReadLine()?.Trim().ToUpper();

            TemplateVariableUpdate = string.IsNullOrEmpty(answer) || answer is "Y" or "YES";

            if (!TemplateVariableUpdate) return;
        }

        if (TemplateVariableUpdate || GlobalVariableUpdate)
        {
            if (TemplateVariableUpdate && string.IsNullOrEmpty(TemplateName))
            {
                Logger.LogError("Need the name of the template to update his variables");
                return;
            }

            var variableFile = TemplateVariableUpdate ? VariableFile.Template : VariableFile.Global;
            SetVariableCmdController.LaunchVariableSetting(variableFile);
            return;
        }

        if (string.IsNullOrEmpty(TemplateName))
        {
            MenuCmdController.StartMenu();
            return;
        }


        Cast.LaunchCast();
    }
}