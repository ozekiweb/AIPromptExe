using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AIPrompt
{
    internal static class Logger
    {
        public enum LogLevel { Error, Debug }

        public static LogLevel logLevel = LogLevel.Debug;

        public static void setVerbosity(bool verbose)
        {
            if (verbose)
            {
                logLevel = LogLevel.Debug;
            }
            else
            {
                logLevel = LogLevel.Error;
            }
        }

        public static void Error(string error)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetTime());
            sb.Append(" [ERROR]: ");
            sb.Append(error);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(sb.ToString());
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Debug(string error)
        {
            if (logLevel < LogLevel.Debug)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(GetTime());
            sb.Append(" [DEBUG] ");
            sb.Append(error);
            Console.WriteLine(sb.ToString());
        }      

        private static string GetTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
        }
    }
}
