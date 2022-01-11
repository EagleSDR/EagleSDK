using LibEasyCrossPlatform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EagleWebSDK
{
    public static class EagleGlobal
    {
        public static bool HideCli { get; set; } = false;

        public static DirectoryInfo GetGlobalDir()
        {
            return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).GetDirectory("RaptorSDK", true);
        }

        public static ECPEnviornment GetGlobalPrefix(string platform)
        {
            return new ECPEnviornment(GetGlobalDir().GetDirectory("prefix", true).GetDirectory(platform, true));
        }
    }
}
