namespace ScriptCaster.Services;

public static class Logger
{
    public static void Log(string message, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void LogWarning(string message)
    {
        Log(message, ConsoleColor.Yellow);
    }

    public static void LogError(string message)
    {
        Log(message, ConsoleColor.Red);
    }

    public static void LogSuccess(string message)
    {
        Log(message, ConsoleColor.Green);
    }
}