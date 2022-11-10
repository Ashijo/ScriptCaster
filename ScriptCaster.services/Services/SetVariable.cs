using System.Diagnostics;
using System.Text.Json;

namespace ScriptCaster.Services.Services;

public class SetVariable
{
    public SetVariable(string? variablePath)
    {
        Debug.Assert(variablePath != null, "variablePath == null in SetVariable");
        Path = variablePath;
        try
        {
            ActualVariables = JsonSerializer.Deserialize<Dictionary<string, string>?>(
                                  File.OpenRead(Path))
                              ?? new Dictionary<string, string>();
        }
        catch (JsonException)
        {
            ActualVariables = new Dictionary<string, string>();
        }

        VariablesBuffer = new Dictionary<string, string>(ActualVariables);
    }

    private Dictionary<string, string> ActualVariables { get; set; }
    private Dictionary<string, string> VariablesBuffer { get; }
    private string Path { get; }

    /// <summary>
    ///     Add a variable in the buffer
    /// </summary>
    /// <param name="name">The key of the variable</param>
    /// <param name="value">The value of the variable</param>
    /// <returns>true if the element is successfully added; False if the key already existed</returns>
    public bool AddVariableToBuffer(string name, string value)
    {
        if (VariablesBuffer.ContainsKey(name)) return false;
        VariablesBuffer.Add(name, value);
        return true;
    }

    /// <summary>
    ///     Remove a variable from the buffer
    /// </summary>
    /// <param name="name">The key of the variable that must be removed</param>
    /// <returns>
    ///     true if the element is successfully found and removed; otherwise, false. This method returns false if key is
    ///     not found in the Buffer
    /// </returns>
    public bool RemoveVariableToBuffer(string name)
    {
        return VariablesBuffer.Remove(name);
    }

    /// <summary>
    ///     Edit a variable in the buffer
    /// </summary>
    /// <param name="name">The key</param>
    /// <param name="value">The value</param>
    /// <returns>False if key doesn't exist</returns>
    public bool EditVariableToBuffer(string name, string value)
    {
        if (!VariablesBuffer.ContainsKey(name)) return false;

        VariablesBuffer[name] = value;

        return true;
    }

    public Dictionary<string, string> GetBuffer()
    {
        return new Dictionary<string, string>(VariablesBuffer);
    }

    public Dictionary<string, string> GetActualVariables()
    {
        return new Dictionary<string, string>(ActualVariables);
    }

    public Dictionary<string, string> WriteBuffer()
    {
        var jsonBufferVariable = JsonSerializer.Serialize(VariablesBuffer);
        File.WriteAllText(Path, jsonBufferVariable);

        ActualVariables = JsonSerializer.Deserialize<Dictionary<string, string>?>(
                              File.OpenRead(Path))
                          ?? new Dictionary<string, string>();

        return new Dictionary<string, string>(ActualVariables);
    }
}