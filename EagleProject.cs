using EagleWeb.Package.Manifest;
using EagleWebSDK.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EagleWebSDK
{
    public class EagleProject
    {
        public EagleProject(DirectoryInfo root)
        {
            this.root = root;
            config = JsonConvert.DeserializeObject<ProjectConfig>(File.ReadAllText(ConfigFilePath));
        }

        private DirectoryInfo root;
        private ProjectConfig config;
        private List<EagleManifestObjectNative> manifestObjectNatives = new List<EagleManifestObjectNative>();

        public DirectoryInfo PathRoot => root;
        public DirectoryInfo PathAssets => root.GetDirectory("assets", true);
        public DirectoryInfo PathBuild => root.GetDirectory("build", true);
        public DirectoryInfo PathBuildManaged => PathBuild.GetDirectory("managed", true);
        public DirectoryInfo PathOut => root.GetDirectory("out", true);
        public ProjectConfig Config => config;
        public EagleManifestObjectNative[] ManifestObjectNatives => manifestObjectNatives.ToArray();
        private string ConfigFilePath => root.GetFilePath("eagleplugin.json");
        private string BuildIdPath => root.GetFilePath("build_id");

        public long BuildIndex
        {
            get
            {
                if (File.Exists(BuildIdPath))
                    return long.Parse(File.ReadAllText(BuildIdPath));
                else
                    return 0;
            }
            set
            {
                File.WriteAllText(BuildIdPath, value.ToString());
            }
        }

        public void SaveConfig()
        {
            File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public DirectoryInfo GetBuildObjectPath(string platform, string objectName)
        {
            return PathBuild.GetDirectory(platform, true).GetDirectory("OBJ-" + objectName, true);
        }

        public DirectoryInfo GetBuildOutPath(string platform)
        {
            return PathBuild.GetDirectory(platform, true).GetDirectory("OUT", true);
        }

        public Dictionary<string, DirectoryInfo> GetBuildOutPaths()
        {
            //Create dict that stores the platform:directory
            Dictionary<string, DirectoryInfo> result = new Dictionary<string, DirectoryInfo>();

            //Find
            DirectoryInfo[] platforms = PathBuild.GetDirectories();
            foreach (var p in platforms)
            {
                DirectoryInfo output = p.GetDirectory("OUT");
                if (output.Exists)
                    result.Add(p.Name, output);
            }

            return result;
        }

        public void RegisterManifestObject(EagleManifestObjectNative obj)
        {
            manifestObjectNatives.Add(obj);
        }
    }
}
