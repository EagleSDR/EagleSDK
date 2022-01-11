using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EagleWebSDK.CLI
{
    public static class CommandCreateWeb
    {
        public static int Run(CLIReader reader)
        {
            //Open project
            EagleProject project = new EagleProject(reader.ProjectPath);

            //Make sure there isn't already a web folder
            DirectoryInfo web = new DirectoryInfo(reader.ProjectPath.FullName + Path.DirectorySeparatorChar + "web");
            if (web.Exists)
                throw new Exception("Web already created. Aborting!");

            //Create directories
            web.Create();
            DirectoryInfo src = web.CreateSubdirectory("src");

            //Unpack templates
            UnpackTemplate(web, "package.json", "template-web-package", new Dictionary<string, string>()
            {
                {"NAME", project.Config.name },
                {"AUTHOR", project.Config.developer }
            });
            UnpackTemplate(web, "tsconfig.json", "template-web-tsconfig");
            UnpackTemplate(web, "webpack.config.js", "template-web-webpack");
            UnpackTemplate(src, "PluginMain.ts", "template-web-main");
            UnpackTemplate(src, "index.js", "template-web-index", new Dictionary<string, string>()
            {
                {"NAME", project.Config.name },
                {"TYPE", project.Config.modules[0].classname }
            });

            //Clone the SDK
            SDKUtils.RunCLI("git", "clone https://github.com/EagleSDR/WebSDK.git " + web.GetDirectory("sdk").FullName);

            //Add command
            var cmds = project.Config.post_commands.ToList();
            cmds.Add(new Config.ProjectConfigCommand
            {
                command = "npx webpack",
                working_directory = "%ROOT%/web",
                platforms = new string[] { "*" }
            });
            project.Config.post_commands = cmds.ToArray();
            project.SaveConfig();

            return 0;
        }

        private static void UnpackTemplate(DirectoryInfo dst, string name, string templateName, Dictionary<string, string> replacements = null)
        {
            //Load
            string data = SDKUtils.LoadTemplate(templateName);

            //Do replacements
            if (replacements != null)
            {
                foreach (var r in replacements)
                    data = data.Replace("%" + r.Key + "%", r.Value);
            }

            //Write
            string path = dst.FullName + Path.DirectorySeparatorChar + name;
            File.WriteAllText(path, data);
        }
    }
}
