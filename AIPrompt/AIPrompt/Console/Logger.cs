using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;


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

        public static void Error(params string[] error)
        {
            foreach (string s in error)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetTime());
                sb.Append(" [ERROR]: ");
                Console.ForegroundColor = ConsoleColor.Red;     
                Console.Write(sb.ToString());
                PrintMessageWithPadding(s);
            }
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
            Console.Write(sb.ToString());
            PrintMessageWithPadding(error);
        }

        //Enter padding at the beginning of each line
        private static void PrintMessageWithPadding(string message)
        {
            if (message == null)
            {
                return;
            }
          
            var splitted = message.Split(Environment.NewLine);
            const string padding = "  ";
            var width = Console.WindowWidth;
            var list = new List<string>
            {
                splitted[0]
            };
            for (int i = 1; i < splitted.Length; i++)
            {
                if (splitted[i].Length < width)
                {
                    list.Add(padding + splitted[i]);
                    continue;
                }
                var split = splitted[i].Split(" ");
                var line = padding;
                foreach (var v in split)
                {
                    if (v.Length + line.Length < width - padding.Length)
                    {
                        line += v + " ";
                    }
                    else
                    {
                        list.Add(line);
                        line = padding;
                        line += v;
                    }
                }
            }

            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
        }

        private static string GetTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
        }       
    }
}
