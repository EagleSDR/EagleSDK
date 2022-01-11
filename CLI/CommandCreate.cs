using EagleWebSDK.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace EagleWebSDK.CLI
{
    static class CommandCreate
    {
        public static int Run(CLIReader reader)
        {
            //Get the path to the EagleWeb.Common.csproj
            reader.GetSDKCommonProject();

            //Prompt user for options
            string developer = MakeIdAcceptable(reader.Prompt("Type the developer name.", false));
            string name = MakeIdAcceptable(reader.Prompt("Type the plugin name.", false));
            string module = MakeIdAcceptable(reader.Prompt("Type the module name. This doesn't matter too much.", false));

            //Create the directory name
            DirectoryInfo dir = reader.ProjectPath.CreateSubdirectory($"{developer}.{name}");
            reader.WriteLine($"Creating in \"{dir.FullName}\"...");

            //Determine some bits
            string managedNamespace = $"{developer}.{name}";
            string managedClassname = $"{module}Module";
            string managedFullClassname = managedNamespace + "." + managedClassname;
            string managedPath = $"managed/{managedNamespace}/";
            string nativeName = $"native-{developer}-{name}";
            string nativePath = $"native/{nativeName}/";

            //Write the CS project
            DirectoryInfo managedDirectory = dir.CreateSubdirectory("managed").CreateSubdirectory(managedNamespace);
            File.WriteAllText(managedDirectory.GetFilePath(managedNamespace + ".csproj"), LoadResource("template-csproj.txt"));
            File.WriteAllText(managedDirectory.GetFilePath(managedClassname + ".cs"), LoadResource("template-cs.txt")
                .Replace("%NAMESPACE%", managedNamespace)
                .Replace("%CLASSNAME%", managedClassname)
                .Replace("%MODULE%", module)
            );

            //Write the native project
            DirectoryInfo nativeDirectory = dir.CreateSubdirectory("native").CreateSubdirectory(nativeName);
            File.WriteAllText(nativeDirectory.GetFilePath("native.h"), LoadResource("template-native-h.txt"));
            File.WriteAllText(nativeDirectory.GetFilePath("native.cpp"), LoadResource("template-native-cpp.txt"));
            File.WriteAllText(nativeDirectory.GetFilePath("CMakeLists.txt"), LoadResource("template-native-cmake.txt")
                .Replace("%NAME%", nativeName)
            );

            //Create and write the config file
            File.WriteAllText(dir.GetFilePath("eagleplugin.json"), JsonConvert.SerializeObject(new ProjectConfig
            {
                developer = developer,
                name = name,
                version_major = 0,
                version_minor = 0,
                modules = new EagleWeb.Package.Manifest.EagleManifestModule[]
                {
                    new EagleWeb.Package.Manifest.EagleManifestModule
                    {
                        classname = managedFullClassname,
                        dll = managedNamespace + ".dll"
                    }
                },
                objects = new ProjectConfigObjects
                {
                    dotnet = new ProjectConfigDotnetObject[]
                    {
                        new ProjectConfigDotnetObject
                        {
                            configuration = "Release",
                            name = managedNamespace,
                            project_path = managedPath
                        }
                    },
                    native = new ProjectConfigNativeObject[]
                    {
                        new ProjectConfigNativeObject
                        {
                            type = "cmake",
                            name = nativeName,
                            path = nativePath,
                            platforms = new string[]
                            {
                                "win-x64",
                                "linux-x64"
                            },
                            extra = new Newtonsoft.Json.Linq.JObject()
                        }
                    }
                }
            }, Formatting.Indented));

            //Log
            reader.WriteLine("Success!", ConsoleColor.Green);

            return 0;
        }

        static string MakeIdAcceptable(string input)
        {
            while (input.Contains(" "))
                input = input.Replace(" ", "");
            return input;
        }

        static string LoadResource(string name)
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("EagleWebSDK.Templates." + name))
            using (StreamReader sr = new StreamReader(s))
                return sr.ReadToEnd();
        }
    }
}
