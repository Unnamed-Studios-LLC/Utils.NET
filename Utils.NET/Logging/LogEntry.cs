using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.NET.Logging
{
    public class LogWrite
    {
        public string text;
        public ConsoleColor color;

        public LogWrite(string text, ConsoleColor color)
        {
            this.text = text;
            this.color = color;
        }
    }

    public class LogEntry
    {
        public static LogEntry Init(object obj) => Init(obj.ToString(), ConsoleColor.White);
        public static LogEntry Init(object obj, ConsoleColor color) => Init(new LogWrite(obj.ToString(), color));
        public static LogEntry Init(string text) => Init(text, ConsoleColor.White);
        public static LogEntry Init(string text, ConsoleColor color) => Init(new LogWrite(text, color));
        public static LogEntry Init(LogWrite write) => new LogEntry().Append(write);


        public List<LogWrite> writes = new List<LogWrite>();

        public LogEntry Append(string text) => Append(text, ConsoleColor.White);
        public LogEntry Append(string text, ConsoleColor color) => Append(new LogWrite(text, color));
        public LogEntry Append(LogWrite write)
        {
            writes.Add(write);
            return this;
        }
    }
}
