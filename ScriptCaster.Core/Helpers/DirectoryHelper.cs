namespace ScriptCaster.Core.Helpers;

public static class DirectoryHelper
{
	public static string[] GetDirectoriesName(string path)
	{
		return Directory.GetDirectories(path)
			.Select(p => new DirectoryInfo(p).Name)
			.ToArray();
	}

	//TODO: define config filenames (.variable.json and .config) as variable in user .config directory
	public static string[] GetAllFilesWithoutConfigs(string path)
	{
		return Directory.GetFiles(path)
			.Where(n => !n.Contains(".variables.json") && !n.Contains(".config"))
			.ToArray();
	}
}
