using System.Diagnostics;
using System.Text.Json;
using Fluid;
using ScriptCaster.Core.Callbacks;
using ScriptCaster.Core.Extensions;

namespace ScriptCaster.Core.Services;

public static partial class Cast
{
	public static RLaunchCastCallback LaunchCast(bool force)
	{
		if (!Context.Initialized)
		{
			return new RLaunchCastCallback(false, false);
		}

		var variablesCallback = GetVariables();

		if (!variablesCallback.Success)
		{
			return new RLaunchCastCallback(false, true, variablesCallback);
		}

		var fluidModel = variablesCallback.Variables!;

		var context = new TemplateContext(fluidModel);

		var templateFolders = GetAllFolders();
		var filesInTemplate = GetAllFiles(templateFolders);

		CreateFoldersNew(context, fluidModel, templateFolders);
		CreateFilesNew(context, filesInTemplate, force);

		return new RLaunchCastCallback(true, true);
	}

	private static RValidateVariablesCallback GetVariables()
	{
		Debug.Assert(Context.GlobalVariablePath != null, "GlobalVariablePath is null in Cast.LaunchCast");
		var variables = JsonSerializer.Deserialize<Dictionary<string, string>>(
			File.ReadAllText(Context.GlobalVariablePath));

		Debug.Assert(Context.TemplateVariablePath != null, "TemplateVariablePath is null in Cast.LaunchCast");
		var templateVariables = JsonSerializer.Deserialize<Dictionary<string, string>>(
			File.ReadAllText(Context.TemplateVariablePath));

		var validationCallback = ValidateVariables(variables, templateVariables);
		if (!validationCallback.Success)
		{
			return validationCallback;
		}

		Debug.Assert(variables != null && templateVariables != null, "ValidateVariables() failed");
		variables.AddRangeOverride(templateVariables); // Global variables can be override by local variables

		return new RValidateVariablesCallback(true, true, true, variables);
	}

	
	private static void CreateFoldersNew(TemplateContext fluidContext, Dictionary<string, string> variables, string[] templatesFolders)
	{
		var parser = new FluidParser();

		foreach (var templateFolder in templatesFolders)
		{
			Debug.Assert(Context.TemplatePath != null, "TemplatePath is null in Cast.CreateFolders");
			
			var resultFolder = FluidParse(fluidContext, parser, templateFolder)
				.Replace("\n", "");

			for (var i = 0; i < Context.RecursionLevel; i++)
			{
				resultFolder = FluidParse(fluidContext, parser, templateFolder);
			}

			Directory.CreateDirectory(resultFolder);
		}
	}
	
	private static void CreateFilesNew(TemplateContext fluidContext, string[] filesInTemplate,
		bool forced = false)
	{
		var parser = new FluidParser();

		foreach (var tFilePath in filesInTemplate)
		{
			Debug.Assert(Context.TemplatePath != null, "TemplatePath is null in Cast.LaunchCast");

			var newFilePath = FluidParse(fluidContext, parser, tFilePath);
			
			for (var i = 0; i < Context.RecursionLevel - 1; i++) 
			{
				newFilePath = FluidParse(fluidContext, parser, tFilePath);
			}

			//TODO: verify if this is mandatory, I think yes but...
			newFilePath = newFilePath.Replace("\n", "");
			
			var resultFileExist = File.Exists(newFilePath);

			if (resultFileExist && !forced)
			{
				if (!forced)
				{
					Console.WriteLine(
						$"{newFilePath} already exist. Ignored. You can force replace with -f or --force.");
					continue;
				}
			}

			if (resultFileExist)
			{
				Directory.Delete(newFilePath);
			}

			var fileContent = File.ReadAllText(tFilePath);
			
			for (var i = 0; i < Context.RecursionLevel - 1; i++)
			{
				fileContent = FluidParse(fluidContext, parser, fileContent);
			}

			using var stream = File.CreateText(newFilePath);
			stream.Write(fileContent);
			stream.Close();
		}
	}

	private static string FluidParse(TemplateContext fluidContext, FluidParser parser, string fileContent)
	{
		string? result = null;
		if (parser.TryParse(fileContent, out var fluidResult, out var error))
		{
			result = fluidResult.Render(fluidContext);
		}
		
		//TODO: manage exceptions
		
		return result ?? "";
	}
}
