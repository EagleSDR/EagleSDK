using EagleWeb.Package.Manifest;
using EagleWebSDK.Builders;
using LibEasyCrossPlatform;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace EagleWebSDK.CLI
{
    public static class CommandBuild
    {
        public static void Run(CLIReader reader)
        {
            //Open project
            EagleProject project = new EagleProject(reader.ProjectPath);
            long buildId = project.BuildIndex++;

            //Log
            Console.WriteLine($"Beginning build of \"{project.Config.name}\" by \"{project.Config.developer}\"... (build ID {buildId})");

            //Build CSharp objects
            foreach (var o in project.Config.objects.dotnet)
                DotnetObjectBuilder.Build(project, o);

            //Build native objects
            foreach (var o in project.Config.objects.native)
                NativeObjectBuilder.Build(project, o);

            //Run post-build commands
            foreach (var c in project.Config.post_commands)
            {
                string dir = c.working_directory.Replace("%ROOT%", project.PathRoot.FullName);
                foreach (var p in c.platforms)
                {
                    string modifiedArgs = c.command
                        .Replace("%OUTPUT%", p == "*" ? "" : ECPUtils.EscapeCmd(project.GetBuildOutPath(p)))
                        .Replace("%ROOT%", ECPUtils.EscapeCmd(project.PathRoot.FullName));
                    Console.WriteLine($"Running post-build command (under bash): {modifiedArgs}");
                    Console.WriteLine($"At: {dir}");
                    SDKUtils.RunBashCLI(modifiedArgs, dir);
                }
            }

            //Get the output filename after removing any files in the output
            foreach (FileInfo file in project.PathOut.GetFiles())
                file.Delete();
            string outputFilename = project.PathOut.GetFilePath($"{project.Config.developer}.{project.Config.name}-v{project.Config.version_major}.{project.Config.version_minor}.{buildId}.egk");

            //Begin packaging the output
            Console.WriteLine("Creating output package...");
            using (FileStream fs = new FileStream(outputFilename, FileMode.Create))
            using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                //First, copy all native objects
                NativeObjectBuilder.MoveOutputToZip(project, zip, "native/");

                //Copy all to the ZIP
                SDKUtils.WriteDirectoryToZip(project.PathBuildManaged, zip, "managed/");

                //Copy assets to ZIP
                SDKUtils.WriteDirectoryToZip(project.PathAssets, zip, "assets/");

                //Create the manifest
                EagleManifest manifest = new EagleManifest
                {
                    plugin_name = project.Config.name,
                    developer_name = project.Config.developer,
                    version_major = project.Config.version_major,
                    version_minor = project.Config.version_minor,
                    version_build = (int)buildId,
                    build_at = DateTime.UtcNow,
                    build_platform = Environment.OSVersion.VersionString,
                    sdk_version_major = Program.VERSION_MAJOR,
                    sdk_version_minor = Program.VERSION_MINOR,
                    objects_native = project.ManifestObjectNatives,
                    modules = project.Config.modules
                };

                //Write manifest
                using (Stream manifestStream = zip.CreateEntry("eagle-manifest.json").Open())
                    manifestStream.WriteString(JsonConvert.SerializeObject(manifest, Formatting.Indented));
            }
            Console.WriteLine($"Build success! Package written to \"{outputFilename}\".");
        }
    }
}
