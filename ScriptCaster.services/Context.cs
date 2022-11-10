using ScriptCaster.Services.Helpers;

namespace ScriptCaster.Services;

//TODO: Change it from singleton to static. 
public class Context
{
    public string? TemplateName { get; private set; }
    public string? TemplatePath { get; private set; }
    public string? TemplateVariablePath { get; private set; }
    public string? LocalPath { get; private set; }
    public string? GlobalVariablePath { get; private set; }
    public int? Recursivity { get; private set; }
    public bool Forced { get; private set; }
    public bool Initiated { get; private set; }

    //TODO: the default template path shall also be a config in ~/.config/ScriptCaster/config
    //TODO: Default recursivity shall also be a config
    public void InitContext(string? templateName, string templatesCollectionPath, int recursivity, bool forced,
        bool created)
    {
        if (templateName == null) return;

        TemplateName = templateName;

        if (!Directory.Exists(templatesCollectionPath))
        {
            Logger.LogError($"{templatesCollectionPath} does not exist");
            Logger.LogError("   this is the folder that should contain all the templates");
            Logger.LogError("   please create the folder then try create a new template with :");
            Logger.Log($"      sc {templateName} -c", ConsoleColor.Cyan);
            return;
        }

        TemplatePath = $"{templatesCollectionPath}/{templateName}";

        if (!Directory.Exists(TemplatePath) && !created)
        {
            Logger.LogError($"{TemplatePath} does not exist");
            Logger.LogError("   this is the folder that contain the template and local information about it");
            Logger.LogError("   You may want to create a new template, here is the command : ");
            Logger.Log($"      sc {templateName} -c", ConsoleColor.Cyan);
            return;
        }


        TemplateVariablePath = $"{TemplatePath}/.variables.json";

        LocalPath = Directory.GetCurrentDirectory();

        GlobalVariablePath = $"{templatesCollectionPath}/.variables.json";
        Recursivity = recursivity;
        Forced = forced;

        Initiated = true;
    }

    public static void ListTemplates(string templatesCollectionPath)
    {
        var templateList = DirectoryHelper.GetDirectoriesName(templatesCollectionPath);

        Console.WriteLine($"I found {templateList.Count()} template{(templateList.Count() > 1 ? "s" : "")} :");

        foreach (var template in templateList) Console.WriteLine($"   - {template}");

        if (templateList.Any()) return;

        Console.WriteLine("   Do not forget that only folders will be considered as template.");
        Console.WriteLine("If you want just a file, touch it in a folder");
    }

    #region Singleton

    private static Context? _instance;

    private Context()
    {
        Initiated = false;
    }

    public static Context Instance => _instance ??= new Context();

    #endregion
}