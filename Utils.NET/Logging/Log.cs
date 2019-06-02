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
        /// Logs the string representation of the given object
        /// </summary>
        /// <param name="line"></param>
        public static void Error(object obj)
        {
            Error(obj.ToString());
        }

        /// <summary>
        /// Logs a line
        /// </summary>
        /// <param name="line"></param>
        public static void Error(string line)
        {
            Write(line, ConsoleColor.Red);
        }

        /// <summary>
        /// Logs the string representation of the given object
        /// </summary>
        /// <param name="line"></param>
        public static void Write(object obj)
        {
            Write(obj.ToString());
        }

        /// <summary>
        /// Logs the string representation of the given object
        /// </summary>
        /// <param name="line"></param>
        public static void Write(object obj, ConsoleColor color)
        {
            Write(obj.ToString(), color);
        }

        /// <summary>
        /// Logs a line
        /// </summary>
        /// <param name="line"></param>
        public static void Write(string line)
        {
            Write(line, ConsoleColor.White);
        }

        /// <summary>
        /// Logs a line
        /// </summary>
        /// <param name="line"></param>
        public static void Write(string line, ConsoleColor color)
        {
            Write(LogEntry.Init(line, color));
        }

        /// <summary>
        /// Logs an entry
        /// </summary>
        /// <param name="line"></param>
        public static void Write(LogEntry entry)
        {
            if (Instance.running)
                Instance.LogLine(entry);
            else
                Instance.WriteEntry(entry);
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
        private ConcurrentQueue<LogEntry> logLines = new ConcurrentQueue<LogEntry>();

        /// <summary>
        /// Event used to delay input checking
        /// </summary>
        private ManualResetEvent delayEvent = new ManualResetEvent(false);

        /// <summary>
        /// Bool determining if the Log is running of not
        /// </summary>
        private bool running = false;

        /// <summary>
        /// The current input line
        /// </summary>
        private string inputLine = "";

        public Log()
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        /// <summary>
        /// Runs the input loop
        /// </summary>
        private void RunLog()
        {
            running = true;
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
                    var c = key.KeyChar;
                    if (!char.IsLetterOrDigit(c)) return;
                    input += c;
                    WriteInputKey(c);
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
            if (string.IsNullOrWhiteSpace(input)) return; // empty command
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
            LogEntry entry;
            bool first = true;
            while (logLines.TryDequeue(out entry))
            {
                if (first)
                {
                    ClearCurrentLine();
                    first = false;
                }
                WriteEntry(entry);
            }

            if (inputLine.Length > 0 && !first)
                Console.Write(inputLine);
        }
        
        /// <summary>
        /// Writes an entry into the log prefixed with a timestamp
        /// </summary>
        /// <param name="line"></param>
        private void WriteEntry(LogEntry entry)
        {
            WriteTimestamp();
            foreach (var write in entry.writes)
                Write(write);
            Console.Write('\n');
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void Write(LogWrite write)
        {
            Console.ForegroundColor = write.color;
            Console.Write(write.text);
        }

        private void WriteTimestamp()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write('[');
            Console.Write(DateTime.Now);
            Console.Write("] ");
        }

        /// <summary>
        /// Clears the current input line of the console
        /// </summary>
        private void ClearCurrentLine()
        {
            Console.Write("\r" + new string(' ', Console.WindowWidth - 2) + "\r");
        }

        /// <summary>
        /// Logs an entry within the console
        /// </summary>
        /// <param name="line"></param>
        public void LogLine(string line)
        {
            LogLine(LogEntry.Init(line));
        }

        /// <summary>
        /// Logs an entry within the console
        /// </summary>
        /// <param name="line"></param>
        public void LogLine(LogEntry entry)
        {
            logLines.Enqueue(entry);
        }

        public void Dispose()
        {
            LogLine("Program Stopping...");
            running = false;
            delayEvent.Set();
        }
    }
}
