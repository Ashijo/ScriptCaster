﻿
using McMaster.Extensions.CommandLineUtils;
using ScriptCaster.app;


[Command(Name = "ScriptCaster", Description = "Grab a folder, replace variables paste it wherever you are")]
[HelpOption("-?|-h|--help")]
class Program {

    static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);


    [Argument(0, Name = "TemplateName", Description = "The name of the template we have to work with")]
    private string? TemplateName { get; }

    [Option("-t|--template", Description = "Update the variables files of the template (work in progress)")]
    public bool TemplateVariableUpdate { get; } = false;

    [Option("-g|--global", Description = "Update the global variables files (work in progress)")]
    public bool GlobalVariableUpdate { get; } = false;

    [Option(Description = "List of all templates")]
    public bool List { get; } = false;

    //IDEA: Create template from other template ?    
    [Option(Description = "Create a new empty template (work in progress)")]
    public bool Create { get; } = false;

    //TODO: get the default recursive from .config
    [Option(Description = "The number of time parsing will be done on every text, bigger it is the more depth you can have in variables references")]
    public int Recursivity {get;} = 3;

    //TODO: get the default path from .config
    [Option("-p|--path", Description="Path of the folder containing the template")]
    public string TemplatesCollectionPath { get; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Templates";

    void OnExecute() { 

        if(List) {
            Context.Instance.ListTemplates(TemplatesCollectionPath);
            return;
        }

        Context.Instance.InitContext(TemplateName, TemplatesCollectionPath, Recursivity);

        if(TemplateVariableUpdate || GlobalVariableUpdate) {
            if (TemplateVariableUpdate && string.IsNullOrEmpty(TemplateName)) {
                Logger.LogError("Need the name of the template to update his variables");
                return;
            }
            var variableFile = GlobalVariableUpdate ? VariableFile.Global : 0;
            variableFile |= TemplateVariableUpdate ? VariableFile.Template : 0;
            SetVariable.LaunchVariableSetting(variableFile);
            return;
        } 
        
        if (string.IsNullOrEmpty(TemplateName)) {
            Logger.LogError("Need the name of the template to work with");
            return;
        }
        Cast.LaunchCast();
    }
}