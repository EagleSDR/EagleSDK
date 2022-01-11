using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EagleWebSDK
{
    public class CLIReader
    {
        public CLIReader(string[] args)
        {
            this.args = args;
        }

        private string[] args;
        private int index;

        public DirectoryInfo ProjectPath => new DirectoryInfo(Directory.GetCurrentDirectory());

        public bool TryReadNextCommand(out string command)
        {
            if (index < args.Length)
            {
                command = args[index++];
                return true;
            } else
            {
                command = null;
                return false;
            }
        }

        public string GetSDKCommonProject()
        {
            string common = Environment.GetEnvironmentVariable("EAGLESDR_COMMON");
            if (common == null || common == "")
            {
                throw new Exception("Please set the \"EAGLESDR_COMMON\" enviornmental variable to the pathname to your SDK's \"EagleWeb.Common.csproj\" project file.");
            }
            return common;
        }

        public string Prompt(string label, bool allowBlank)
        {
            //Write question
            WriteLine(label, ConsoleColor.Magenta);

            //Enter prompt loop
            while (true)
            {
                //Prompt
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(">");
                string response = Console.ReadLine();

                //Check if it's acceptable
                if (allowBlank || response.Length > 0)
                    return response;

                //Reject
                WriteLine("Please type a response.", ConsoleColor.Yellow);
            }
        }

        public void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
