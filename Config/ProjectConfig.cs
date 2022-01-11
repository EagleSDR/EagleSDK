using EagleWeb.Package.Manifest;
using System;
using System.Collections.Generic;
using System.Text;

namespace EagleWebSDK.Config
{
    public class ProjectConfig
    {
        public string developer;
        public string name;
        public int version_major;
        public int version_minor;
        public ProjectConfigObjects objects;
        public EagleManifestModule[] modules;
        public ProjectConfigCommand[] post_commands = new ProjectConfigCommand[0];
    }
}
