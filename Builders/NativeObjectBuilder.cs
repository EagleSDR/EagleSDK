using EagleWebSDK.Config;
using LibEasyCrossPlatform;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace EagleWebSDK.Builders
{
    public static class NativeObjectBuilder
    {
        public static void Build(EagleProject project, ProjectConfigNativeObject info)
        {
            BuildCMake(project, info);
        }

        public static void MoveOutputToZip(EagleProject project, ZipArchive zip, string prefix)
        {
            foreach (var p in project.GetBuildOutPaths())
                SDKUtils.WriteDirectoryToZip(p.Value, zip, prefix + p.Key + "/");
        }

        private static void BuildCMake(EagleProject project, ProjectConfigNativeObject info)
        {
            //Loop through all
            foreach (string platform in info.platforms)
            {
                //Log
                Console.WriteLine($"Building native object \"{info.name}\" for {platform}...");

                //Get paths
                DirectoryInfo sourcePath = new DirectoryInfo(Path.Combine(project.PathRoot.FullName, info.path));
                DirectoryInfo pathBuild = project.GetBuildObjectPath(platform, info.name);
                DirectoryInfo pathOut = project.GetBuildOutPath(platform);

                //Create extras, if any
                string extra = "";
                if (info.extra != null && info.extra.ContainsKey("additional_options"))
                {
                    JObject extraOptions = (JObject)info.extra["additional_options"];
                    foreach (var v in extraOptions)
                        extra += $"-D{v.Key}={ECPUtils.EscapeCmd((string)v.Value)} ";
                }

                //Run build
                ECPCmakeBuilder cmake = new ECPCmakeBuilder(sourcePath, pathBuild);
                ECPEnviornment envGlobal = EagleGlobal.GetGlobalPrefix(platform);
                ECPEnviornment envOutput = new ECPEnviornment(pathOut);
                cmake.Build(envGlobal, envOutput, SDKUtils.GetPlatform(platform), extra, EagleGlobal.HideCli);

                //Copy the output to the global enviornment
                envOutput.CopyTo(envGlobal);
            }

            //Register
            project.RegisterManifestObject(new EagleWeb.Package.Manifest.EagleManifestObjectNative
            {
                name = info.name,
                platforms = info.platforms
            });
        }
    }
}
