using EagleWeb.Package.Manifest;
using EagleWebSDK.Builders;
using EagleWebSDK.CLI;
using EagleWebSDK.Config;
using LibEasyCrossPlatform;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;

namespace EagleWebSDK
{
    class Program
    {
        public const int VERSION_MAJOR = 1;
        public const int VERSION_MINOR = 0;

        static int Main(string[] args)
        {
            //Create CLI reader
            CLIReader cli = new CLIReader(args);

            //Enter loop
            try
            {
                //Read the command
                cli.TryReadNextCommand(out string command);

                //Switch on command
                switch (command)
                {
                    case "build":
                        CommandBuild.Run(cli);
                        return 0;
                    case "create":
                        return CommandCreate.Run(cli);
                    case "add_web":
                        return CommandCreateWeb.Run(cli);
                    default:
                        cli.WriteLine($"Invalid usage. You must invoke with one of the following commands:", ConsoleColor.Red);
                        cli.WriteLine($"    build : Builds the current project.");
                        return -1;
                }
            } catch (Exception ex)
            {
                cli.WriteLine(ex.Message, ConsoleColor.Red);
                return -1;
            }
        }
    }
}
