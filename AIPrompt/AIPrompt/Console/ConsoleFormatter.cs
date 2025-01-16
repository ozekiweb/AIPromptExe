using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPrompt
{
    internal static class AIPromptConsole
    {
        public static void PrintMessage(Message message, string? title = null)
        {
            if (title != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(title);
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Role: ");
            Console.ForegroundColor= ConsoleColor.White;
            Console.WriteLine(message.Role);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Content: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message.Content + Environment.NewLine);
        }

        public static void PrintMessage(IEnumerable<Message> messages)
        {
            int i = 1;
            foreach (Message message in messages)
            {
                PrintMessage(message, "Message #" + i++ + ":");             
            }
        }

        public static string Read(string readMessage)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(readMessage + " ");
            Console.ForegroundColor= ConsoleColor.White;
            string? text = Console.ReadLine() ?? AIPromptConsole.Read("Nothing was entered. Enter your prompt again:");
            return text;
        }

        public static void WriteLine(string line)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
