using System.Diagnostics;
using ScriptCaster.Services.Helpers;

namespace ScriptCaster.Services;

public static class Context
{
    public static string? TemplatesCollectionPath { get; private set; }
    public static string? TemplateName { get; private set; }
    public static string? TemplatePath => $"{TemplatesCollectionPath}/{TemplateName}";
    public static string? TemplateVariablePath => $"{TemplatePath}/.variables.json";
    public static string? LocalPath { get; private set; }
    public static string? GlobalVariablePath => "{TemplatesCollectionPath}/.variables.json";
    public static int? Recursivity { get; private set; }
    public static bool Forced { get; private set; }
    public static bool Initiated { get; private set; }

    //TODO: the default template path shall also be a config in ~/.config/ScriptCaster/config
    //TODO: Default recursivity shall also be a config
    public static void InitContext(string? templateName, string templatesCollectionPath, int recursivity, bool forced,
        bool created)
    {
        if (templateName == null) return;

        TemplatesCollectionPath = templatesCollectionPath;
        TemplateName = templateName;

        if (!Directory.Exists(templatesCollectionPath))
        {
            Logger.LogError($"{templatesCollectionPath} does not exist");
            Logger.LogError("   this is the folder that should contain all the templates");
            Logger.LogError("   please create the folder then try create a new template with :");
            Logger.Log($"      sc {templateName} -c", ConsoleColor.Cyan);
            return;
        }

        if (!Directory.Exists(TemplatePath) && !created)
        {
            Logger.LogError($"{TemplatePath} does not exist");
            Logger.LogError("   this is the folder that contain the template and local information about it");
            Logger.LogError("   You may want to create a new template, here is the command : ");
            Logger.Log($"      sc {templateName} -c", ConsoleColor.Cyan);
            return;
        }


        LocalPath = Directory.GetCurrentDirectory();

        Recursivity = recursivity;
        Forced = forced;

        Initiated = true;
    }

    public static void ListTemplates()
    {
        Debug.Assert(TemplatesCollectionPath != null, nameof(TemplatesCollectionPath) + " should never be null... But is :<");
        var templateList = DirectoryHelper.GetDirectoriesName(TemplatesCollectionPath);

        Console.WriteLine($"I found {templateList.Count()} template{(templateList.Count() > 1 ? "s" : "")} :");

        foreach (var template in templateList) Console.WriteLine($"   - {template}");

        if (templateList.Any()) return;

        Console.WriteLine("   Do not forget that only folders will be considered as template.");
        Console.WriteLine("If you want to cast file, touch it in a folder");
    }
}