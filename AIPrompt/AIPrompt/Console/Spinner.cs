using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPrompt
{
    class Spinner
    {
        readonly string[] spinnerChars = { "|", "/", "-", "\\" };
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
                Console.Write($"\r{text + " " + spinnerChars[index]} "); // \r brings the cursor back to the start of the line
                index = (index + 1) % spinnerChars.Length;
                Thread.Sleep(100); // Adjust speed of spinner here
            }

            // Clear spinner when done
            Console.Write($"\r");
            for ( int i = 0; i < text.Length + 2; i++)
            {
                Console.Write($" ");
            }
            Console.Write($"\r");
        }
    }
}
