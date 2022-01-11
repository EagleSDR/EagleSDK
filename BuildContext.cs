using EagleWebSDK.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EagleWebSDK
{
    public class BuildContext
    {
        public BuildContext(ProjectConfig cfg)
        {
            this.cfg = cfg;
        }

        private ProjectConfig cfg;

        public string RootPath => Directory.GetCurrentDirectory();

        public void RunCli(string process, string command)
        {
            Process p = Process.Start(new ProcessStartInfo
            {
                Arguments = command,
                FileName = process
            });
            p.WaitForExit();
            if (p.ExitCode != 0)
                throw new Exception($"CLI \"{process}\" returned unsuccessful exit code {p.ExitCode} when invoked with \"{command}\"");
        }

        public string GetNativeBuildDir(string name, string platform)
        {
            string dir = $"build/objects/native/{name}/{platform}/";
            EnsureDirectory(dir);
            return dir;
        }

        private void EnsureDirectory(string pathname)
        {
            DirectoryInfo d = new DirectoryInfo(pathname);
            if (!d.Exists)
                d.Create();
        }
    }
}
