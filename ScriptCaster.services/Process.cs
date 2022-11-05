
namespace ScriptCaster.Services
{
    public static class Process
    {
        public static string[] GetDirectoriesName(string path)
        {
            return Directory.GetDirectories(path)
                .Select(p => new DirectoryInfo(p).Name)
                .ToArray();
        }

        //TODO: define config filenames as variable in user .config directory
        public static string[] GetAllFilesWithoutConfigs(string path)
        {
            return Directory.GetFiles(path)
                .Where(n => !n.Contains(".variables.json") && !n.Contains(".config"))
                .ToArray();
        }
    }
}