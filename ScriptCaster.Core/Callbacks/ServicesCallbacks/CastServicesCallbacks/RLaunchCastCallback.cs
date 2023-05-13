namespace ScriptCaster.Core.Callbacks;

public record RLaunchCastCallback(
	bool Success,
	bool ContextInitialized,
	RValidateVariablesCallback? ValidateVariablesCallback
);
