using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPrompt
{
    internal class Spinner
    {
        Thread spinnerThread;
        bool spinnerRunning = true;
        readonly string text;

        public Spinner(string? text = null)
        {
            spinnerThread = new Thread(Spin);
            spinnerThread.Start();
            if (text != null)
            {
                this.text = text;
            }
            else
            {
                this.text = "Waiting:";
            }
        }
        public void Stop()
        {
            spinnerRunning = false;
            spinnerThread.Join();
        }

        private void Spin()
        {
            string[] spinnerChars = { "|", "/", "-", "\\" };
            int index = 0;

            while (spinnerRunning)
            {
                Console.Write($"\r{text + " " + spinnerChars[index]} "); 
                index = (index + 1) % spinnerChars.Length;
                Thread.Sleep(100); // Speed of spinner
            }

            // Clear spinner and text when done
            Console.Write($"\r");
            for (int i = 0; i < text.Length + 2; i++)
            {
                Console.Write($" ");
            }
            Console.Write($"\r");
        }
    }
}
