using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Utils.NET.Logging
{
    public class Log : IDisposable
    {
        /// <summary>
        /// Static instance used to handle static log calls
        /// </summary>
        private static Log Instance = new Log();

        /// <summary>
        /// Logs a line
        /// </summary>
        /// <param name="line"></param>
        public static void Push(string line)
        {
            Instance.LogLine(line);
        }

        /// <summary>
        /// Runs the log logic
        /// </summary>
        public static void Run()
        {
            Instance.RunLog();
        }

        /// <summary>
        /// Runs the log logic
        /// </summary>
        public static void Stop()
        {
            Instance.Dispose();
        }

        /// <summary>
        /// Lines ready to be logged
        /// </summary>
        private ConcurrentQueue<string> logLines = new ConcurrentQueue<string>();

        /// <summary>
        /// Event used to delay input checking
        /// </summary>
        private ManualResetEvent delayEvent = new ManualResetEvent(false);

        /// <summary>
        /// Bool determining if the Log is running of not
        /// </summary>
        private bool running = true;

        /// <summary>
        /// The current input line
        /// </summary>
        private string inputLine = "";

        /// <summary>
        /// Runs the input loop
        /// </summary>
        private void RunLog()
        {
            while (running)
            {
                delayEvent.WaitOne(50);

                ReadKeys();
                WriteLogLines();
            }
        }

        /// <summary>
        /// Function called by the InputThread to read keys from the console
        /// </summary>
        private void ReadKeys()
        {
            while (Console.KeyAvailable)
            {
                ProcessKey(Console.ReadKey(true)); // process received keys
            }
        }

        /// <summary>
        /// Evaluates the received key and processes it accordingly
        /// </summary>
        private void ProcessKey(ConsoleKeyInfo key)
        {
            string input = inputLine;
            switch (key.Key)
            {
                case ConsoleKey.Backspace:
                    if (input.Length == 0) break;
                    input = input.Substring(0, input.Length - 1);
                    ClearCurrentLine();
                    if (input.Length > 0)
                        Console.Write(inputLine);
                    break;
                case ConsoleKey.Enter:
                    ProcessInput(input);
                    input = "";
                    break;
                default:
                    input += key.KeyChar;
                    WriteInputKey(key.KeyChar);
                    break;
            }
            inputLine = input;
        }

        /// <summary>
        /// Writes the character of the input received to the console
        /// </summary>
        /// <param name="key"></param>
        private void WriteInputKey(char key)
        {
            Console.Write(key);
        }

        /// <summary>
        /// processes a received input line
        /// </summary>
        /// <param name="input"></param>
        private void ProcessInput(string input)
        {
            input = input.Trim();
            LogLine(input);
            switch (input)
            {
                case "stop":
                case "q":
                    Dispose();
                    break;
            }
        }

        /// <summary>
        /// Flushes all available log lines and rewrites the received input
        /// </summary>
        private void WriteLogLines()
        {
            string line;
            bool first = true;
            while (logLines.TryDequeue(out line))
            {
                if (first)
                {
                    ClearCurrentLine();
                    first = false;
                }
                Console.WriteLine(line);
            }

            if (inputLine.Length > 0 && !first)
                Console.Write(inputLine);
        }

        /// <summary>
        /// Clears the current input line of the console
        /// </summary>
        private void ClearCurrentLine()
        {
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
        }

        /// <summary>
        /// Logs a line within the console
        /// </summary>
        /// <param name="line"></param>
        public void LogLine(string line)
        {
            logLines.Enqueue(line);
        }

        public void Dispose()
        {
            LogLine("Program Stopping...");
            running = false;
            delayEvent.Set();
        }
    }
}
