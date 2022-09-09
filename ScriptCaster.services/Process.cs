using System.Linq;


namespace ScriptCaster.Services
{
    public static class Process
    {
        private static string Exec(string cmd, string args = null ,string working = null) {
            var p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = args ?? "";

            if(working != null) {
                //p.StartInfo.UseShellExecute = true;
                p.StartInfo.WorkingDirectory = working;
            }

            p.Start();
            var result = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return result;
        }

        public static string GetPwd() {
            return Exec("pwd");
        }

        //I'd like to have a function that return only files or only folder
        //but ls -d */ seems broke with process
        //TODO: Add a way to define files to ignore in local config
        public static string[] GetAllFilesAndFolders(string directory) {
            var folderList = Exec($"ls", $"-A", directory);

            return folderList
                .Split('\n')
                .Select(s => s.Trim(' '))
                .Where(s => s != "." && s != ".." && s != ".variables.json" && s != "")
                .ToArray();
        }

    }
}