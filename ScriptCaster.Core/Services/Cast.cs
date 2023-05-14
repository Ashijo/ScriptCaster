using System.Diagnostics;
//TODO: Get ride of newtonsoft
using Newtonsoft.Json;
using ScriptCaster.Core.Callbacks;
using ScriptCaster.Core.Helpers;
using ScriptCaster.Core.Extensions;

namespace ScriptCaster.Core.Services;
/*
Cast shall read the template, the local variable file and the global variable file
variable files are json dictionary <string,string>, every occurence of the key shall be replace by the value
TODO: Split on '%'

TODO: Separator defined in global config ?
would need to fork liquid for that... Maybe one day
*/

public static partial class Cast
{
	public static RLaunchCastCallback LaunchOldCast(bool force = false)
	{
		if (!Context.Initialized)
		{
			return new RLaunchCastCallback(false, false);
		}

		Debug.Assert(Context.GlobalVariablePath != null, "GlobalVariablePath is null in Cast.LaunchCast");
		var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(
			File.ReadAllText(Context.GlobalVariablePath));

		Debug.Assert(Context.TemplateVariablePath != null, "TemplateVariablePath is null in Cast.LaunchCast");
		var templateVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(
			File.ReadAllText(Context.TemplateVariablePath));

		var validationCallback = ValidateVariables(variables, templateVariables);
		if (!validationCallback.Success)
		{
			return new RLaunchCastCallback(false, true, validationCallback);
		}

		Debug.Assert(variables != null && templateVariables != null, "ValidateVariables() failed");
		variables.AddRangeOverride(templateVariables); // Global variables can be override by local variables

		var templateFolders = GetAllFolders();
		var filesInTemplate = GetAllFiles(templateFolders);

		CreateFolders(variables, templateFolders);
		CreateFiles(variables, filesInTemplate, force);

		return new RLaunchCastCallback(true, true);
	}

	private static void CreateFolders(Dictionary<string, string> variables, string[] templatesFolders)
	{
		foreach (var templateFolder in templatesFolders)
		{
			Debug.Assert(Context.TemplatePath != null, "TemplatePath is null in Cast.CreateFolders");
			var resultFolder = templateFolder
				.Replace(Context.TemplatePath, Context.LocalPath)
				.Replace("\n", "");

			for (var i = 0; i < Context.RecursionLevel; i++)
			{
				resultFolder = variables.Aggregate(resultFolder,
					(current, kv) => current.Replace(kv.Key, kv.Value));
			}

			Directory.CreateDirectory(resultFolder);
		}
	}

	private static void CreateFiles(Dictionary<string, string>? variables, string[] filesInTemplate,
		bool forced = false)
	{
		foreach (var tFilePath in filesInTemplate)
		{
			Debug.Assert(Context.TemplatePath != null, "TemplatePath is null in Cast.LaunchCast");
			var resultFilePath = tFilePath
				.Replace(Context.TemplatePath, Context.LocalPath)
				.Replace("\n", "");

			var resultFileExist = File.Exists(resultFilePath);

			if (resultFileExist && !forced)
			{
				if (!forced)
				{
					Console.WriteLine(
						$"{resultFilePath} already exist. Ignored. You can force replace with -f or --force.");
					continue;
				}
			}

			if (resultFileExist)
			{
				Directory.Delete(resultFilePath);
			}

			var fileContent = File.ReadAllText(tFilePath);


			for (var i = 0; i < Context.RecursionLevel; i++)
			{
				Debug.Assert(variables != null, "variables is null");
				foreach (var kv in variables)
				{
					resultFilePath = resultFilePath.Replace(kv.Key, kv.Value);
					fileContent = fileContent.Replace(kv.Key, kv.Value);
				}
			}

			using var stream = File.CreateText(resultFilePath);
			stream.Write(fileContent);
			stream.Close();
		}
	}

	private static string[] GetAllFolders()
	{
		Debug.Assert(Context.TemplatePath != null, "TemplatePath is null in Cast.GetAllFolders");
		return GetTemplateFoldersFromParent(Context.TemplatePath);
	}

	private static string[] GetTemplateFoldersFromParent(string directory)
	{
		var list = new List<string>();
		var foldersInDir = Directory.GetDirectories(directory);

		foreach (var folder in foldersInDir)
		{
			list.Add(folder);
			list.AddRange(GetTemplateFoldersFromParent(folder));
		}

		return list.ToArray();
	}

	//There is a way to merge GetAllFolder and GetAllFile
	//It would be a better optimization
	//But also harder to read
	private static string[] GetAllFiles(string[] templateFolders)
	{
		// BUG: Doesn't get de files in the root folder of template
		
		var files = new List<string>();
		foreach (var folderPath in templateFolders)
		{
			files.AddRange(DirectoryHelper.GetAllFilesWithoutConfigs(folderPath));
		}

		return files.ToArray();
	}

	#region Validations

	private static RValidateVariablesCallback ValidateVariables(Dictionary<string, string>? variables,
		Dictionary<string, string>? templateVariables)
	{
		if (variables != null && templateVariables != null)
		{
			return new RValidateVariablesCallback(true, true, true);
		}

		return new RValidateVariablesCallback(false,
			variables != null,
			templateVariables != null);
	}

	#endregion
}
