using EagleWebSDK.Config;
using LibEasyCrossPlatform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EagleWebSDK.Builders
{
    public static class DotnetObjectBuilder
    {
        public static void Build(EagleProject project, ProjectConfigDotnetObject info)
        {
            //Get the managed path
            string outputPath = project.PathBuildManaged.FullName;

            //Get the path of the project
            string projectPath = Path.Combine(project.PathRoot.FullName, info.project_path);

            //Create CLI args
            string args = $"publish -c {info.configuration} --self-contained --nologo -o {ECPUtils.EscapeCmd(outputPath)} {projectPath}";

            //Run CLI
            Console.WriteLine($"Building managed dotnet object {info.name}...");
            ECPUtils.RunCLI("dotnet", args, EagleGlobal.HideCli);
            Console.WriteLine("Built OK.");
        }
    }
}
