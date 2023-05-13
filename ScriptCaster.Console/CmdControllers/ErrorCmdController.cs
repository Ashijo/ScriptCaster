using System.Text;
using ScriptCaster.Core;
using ScriptCaster.Core.Callbacks;

namespace ScriptCaster.Console.CmdControllers;

public static class ErrorCmdController
{
	public static void PromptCastCallbackError(RLaunchCastCallback castCallback)
	{
		if (!castCallback.ContextInitialized)
		{
			PromptContextNotInitializedError();
		}

		if (castCallback.ValidateVariablesCallback != null)
			PromptValidateVariablesCallbackError(castCallback.ValidateVariablesCallback);
	}

	private static void PromptValidateVariablesCallbackError(RValidateVariablesCallback validateVariablesCallback)
	{
		if(validateVariablesCallback.Success)
			return;
		
		var fileNotExistsInGlobalMsg = $"global folder ({Context.GlobalVariablePath}) ";
		var fileNotExistsInTemplateMsg = $"template folder ({Context.TemplateVariablePath}) ";

		var fileNotExistsInGlobal = !validateVariablesCallback.FileExistsInGlobal;
		var fileNotExistsInTemplate = !validateVariablesCallback.FileExistsInTemplate;
			
		var fileNotExistsIn = fileNotExistsInGlobal
			? fileNotExistsInTemplate
				? $"{fileNotExistsInGlobalMsg} and {fileNotExistsInTemplateMsg}"
				: fileNotExistsInGlobalMsg
			: fileNotExistsInTemplateMsg;

		Logger.LogError($".variables.json does not exists in {fileNotExistsIn}.");
		Logger.LogWarning("You can create it running :");
		var cmdSuggestion = new StringBuilder("   sc ");
		cmdSuggestion.Append(fileNotExistsInTemplate ? "<TemplateName>" : "");
		cmdSuggestion.Append(" -");
		cmdSuggestion.Append(fileNotExistsInGlobal ? "g" : "");
		cmdSuggestion.Append(fileNotExistsInTemplate ? "t" : "");
		
		Logger.Log(cmdSuggestion.ToString(), ConsoleColor.Cyan);
	}

	public static void PromptContextNotInitializedError()
	{
		Logger.LogError("Context not initialized");
	}
}
