namespace ScriptCaster.Core.Callbacks;

public record RValidateVariablesCallback(
	bool Success,
	bool FileExistsInGlobal,
	bool FileExistsInTemplate,
	Dictionary<string, string>? Variables = null);
