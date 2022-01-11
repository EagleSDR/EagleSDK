using LibEasyCrossPlatform;
using LibEasyCrossPlatform.Platforms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace EagleWebSDK
{
    public static class SDKUtils
    {
        public static void RunCLI(string cmd, string args)
        {
            //Start process
            Process p = Process.Start(new ProcessStartInfo
            {
                FileName = cmd,
                Arguments = args,
                RedirectStandardOutput = EagleGlobal.HideCli,
                RedirectStandardError = EagleGlobal.HideCli
            });

            //Wait for it to finish
            p.WaitForExit();

            //If it failed, throw an error
            if (p.ExitCode != 0)
                throw new Exception("Failed to run CLI.");
        }

        public static void RunBashCLI(string cmd, string workingDir)
        {
            //Start process
            Process p = Process.Start(new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/bash",
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"/C {cmd}" : $"-c {ECPUtils.EscapeCmd(cmd)}",
                WorkingDirectory = workingDir,
                RedirectStandardOutput = EagleGlobal.HideCli,
                RedirectStandardError = EagleGlobal.HideCli
            });

            //Wait for it to finish
            p.WaitForExit();

            //If it failed, throw an error
            if (p.ExitCode != 0)
                throw new Exception("Failed to run CLI.");
        }

        public static string GetFilePath(this DirectoryInfo info, string file)
        {
            return info.FullName + Path.DirectorySeparatorChar + file;
        }

        public static DirectoryInfo GetDirectory(this DirectoryInfo info, string name, bool ensure = false)
        {
            DirectoryInfo dir = new DirectoryInfo(info.FullName + Path.DirectorySeparatorChar + name);
            if (ensure)
                ECPUtils.EnsureDirectory(dir);
            return dir;
        }

        public static void WriteString(this Stream stream, string data)
        {
            byte[] payload = Encoding.UTF8.GetBytes(data);
            stream.Write(payload, 0, payload.Length);
        }

        public static IECPPlatform GetPlatform(string platform)
        {
            switch (platform)
            {
                case "win-x64": return new ECPPlatformPrefix("x86_64-w64-mingw32", "Windows");
                case "win-x86": return new ECPPlatformPrefix("x86_64-w32-mingw32", "Windows");
                case "linux-x64": return new ECPPlatformPrefix("x86_64-linux-gnu", "Linux");
                default: throw new Exception($"Unknown platform: {platform}!");
            }
        }

        public static void WriteDirectoryToZip(DirectoryInfo src, ZipArchive zip, string prefix)
        {
            //Get directories and copy
            DirectoryInfo[] dirs = src.GetDirectories();
            foreach (var d in dirs)
                WriteDirectoryToZip(d, zip, prefix + d.Name + "/");

            //Get files and copy
            FileInfo[] files = src.GetFiles();
            foreach (var f in files)
            {
                using (FileStream fSrc = new FileStream(f.FullName, FileMode.Open, FileAccess.Read))
                using (Stream fDst = zip.CreateEntry(prefix + f.Name).Open())
                    fSrc.CopyTo(fDst);
            }
        }

        public static string LoadTemplate(string name)
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("EagleWebSDK.Templates." + name + ".txt"))
            using (StreamReader sr = new StreamReader(s))
                return sr.ReadToEnd();
        }
    }
}
