using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EagleWebSDK.Config
{
    public class ProjectConfigNativeObject
    {
        public string name;
        public string type; //type of project (cmake)
        public string path; //path to source
        public string[] platforms; //platforms to build for
        public JObject extra;
    }
}
