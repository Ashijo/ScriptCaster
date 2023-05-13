namespace ScriptCaster.Core.Callbacks;

public record RSelectTemplateCallback(ESelectTemplateCallbackStatus Result);

public enum ESelectTemplateCallbackStatus
{
	Success,
	NotFound
}
