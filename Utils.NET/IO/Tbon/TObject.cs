using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils.NET.Logging;

namespace Utils.NET.IO.Tbon
{
    public class TObject : TToken, IEnumerable<TObject>
    {
        private enum LineResult
        {
            Success,
            BlankLine,
            EndOfFile
        }

        private enum ValueNameResult
        {
            Success,
            InvalidFormat,
            BlankLine,
            EndOfFile
        }

        private enum ValueResult
        {
            Success,
            ObjectValue,
            EndOfFile
        }

        private class TbonContext
        {
            private bool startOfNextLine = false;

            public StreamReader reader;
            public int line;
            public int character;
            public int tabs;
            public char[] characterBuffer;

            public TbonContext(StreamReader reader)
            {
                this.reader = reader;
                line = 0;
                character = 0;
                characterBuffer = new char[2048];
            }

            public char Read()
            {
                if (startOfNextLine)
                {
                    line++;
                    character = 0;
                    startOfNextLine = false;
                }

                char c = (char)reader.Read();
                character++;
                if (c == '\n')
                {
                    startOfNextLine = true;
                    tabs = 0;
                }
                return c;
            }

            public int Peek() => reader.Peek();
        }


        public override bool IsValue => false;

        public override bool IsObject => true;

        public int ArrayCount
        {
            get
            {
                int c = 0;
                TObject obj = this;
                while (obj != null)
                {
                    obj = obj.nextObject;
                    c++;
                }
                return c;
            }
        }

        public TObject this[int index]
        {
            get
            {
                int c = 0;
                TObject obj = this;
                while (obj != null)
                {
                    if (c == index) return obj;
                    obj = obj.nextObject;
                    c++;
                }
                throw new IndexOutOfRangeException();
            }
        }

        public TToken this[string name]
        {
            get
            {
                return children[name];
            }
        }

        /// <summary>
        /// Dictionary containing the read values and objects within this object
        /// </summary>
        private Dictionary<string, TToken> children = new Dictionary<string, TToken>();

        /// <summary>
        /// The TObject defined after this at the same tab level
        /// </summary>
        private TObject nextObject;

        /// <summary>
        /// Creates a TObject from a given TextReader
        /// </summary>
        /// <param name="reader">Reader.</param>
        public TObject(StreamReader reader)
        {
            Read(new TbonContext(reader), 0);
        }

        /// <summary>
        /// Creates a TObject with a given tabbed in count
        /// </summary>
        private TObject(TbonContext context, int tabCount)
        {
            Read(context, tabCount);
        }

        private void Read(TbonContext context, int tabCount)
        {
            int blankCount = 0;
            while (true)
            {
                ReadTabs(context);
                if (context.tabs == -1) return;
                if (context.tabs != tabCount) return;
                switch (ReadLine(context, tabCount))
                {
                    case LineResult.BlankLine:
                        if (++blankCount == 2)
                        {
                            nextObject = new TObject(context, tabCount);
                            return;
                        }
                        break;
                    case LineResult.EndOfFile:
                        return;
                    case LineResult.Success:
                        blankCount = 0;
                        break;
                }
            }
        }

        private LineResult ReadLine(TbonContext context, int tabCount)
        {
            switch (TryReadValueName(context, out var valueName))
            {
                case ValueNameResult.InvalidFormat:
                    throw new FormatException("Invalid format, " + GetDebugInfo(context));
                case ValueNameResult.BlankLine:
                    return LineResult.BlankLine;
                case ValueNameResult.EndOfFile:
                    return LineResult.EndOfFile;
            }

            switch (TryReadValue(context, out var value))
            {
                case ValueResult.Success:
                    children[valueName] = new TValue(value);
                    break;
                case ValueResult.EndOfFile:
                    throw new FormatException("End of file occured during value discovery, " + GetDebugInfo(context));
                case ValueResult.ObjectValue:
                    children[valueName] = new TObject(context, tabCount + 1);
                    break;
            }

            return LineResult.Success;
        }

        private ValueNameResult TryReadValueName(TbonContext context, out string value)
        {
            ReadUntil(context, out value, out var stopped, '\n', ':');
            switch (stopped)
            {
                case '\0':
                    return ValueNameResult.EndOfFile;
                case '\n':
                    return string.IsNullOrWhiteSpace(value) ? ValueNameResult.BlankLine : ValueNameResult.InvalidFormat;
                case ':':
                    return ValueNameResult.Success;
            }
            return ValueNameResult.InvalidFormat;
        }

        private ValueResult TryReadValue(TbonContext context, out string value)
        {
            ReadUntil(context, out value, out var stopped, '\n');
            switch (stopped)
            {
                case '\0':
                    return string.IsNullOrWhiteSpace(value) ? ValueResult.EndOfFile : ValueResult.Success;
                case '\n':
                    return string.IsNullOrWhiteSpace(value) ? ValueResult.ObjectValue : ValueResult.Success;
            }
            return ValueResult.Success;
        }

        private void ReadUntil(TbonContext context, out string value, out char stopped, params char[] stopChars)
        {
            int charCount = 0;
            char character;
            value = null;
            stopped = '\0';
            while (true)
            {
                int read = context.Peek();
                if (read == -1)
                {
                    stopped = '\0';
                    if (charCount == 0) return;
                    value = new string(context.characterBuffer, 0, charCount);
                    return;
                }
                character = context.Read();
                if (TryCharsContain(stopChars, character, out stopped))
                {
                    if (charCount == 0) return;
                    value = new string(context.characterBuffer, 0, charCount);
                    return;
                }
                else
                {
                    context.characterBuffer[charCount] = character;
                }

                charCount++;
            }
        }

        private bool TryCharsContain(char[] chars, char c, out char stopped)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == c)
                {
                    stopped = chars[i];
                    return true;
                }
            }
            stopped = '\0';
            return false;
        }

        /// <summary>
        /// Reads the tab count for the line
        /// </summary>
        private void ReadTabs(TbonContext context)
        {
            int spaces = 0;
            int peek;
            while (true)
            {
                peek = context.Peek();
                if (peek == -1)
                {
                    context.tabs = -1;
                    return;
                }
                if ((char)peek == '\t')
                {
                    spaces += 4;
                }
                else if ((char)peek == ' ')
                {
                    spaces++;
                }
                else
                {
                    break;
                }
                context.Read();
            }
            context.tabs += (spaces + 3) / 4;
        }

        private string GetDebugInfo(TbonContext context)
        {
            return $"line: {context.line + 1}, char: {context.character + 1}";
        }

        public IEnumerator<TObject> GetEnumerator()
        {
            TObject obj = this;
            while (obj != null)
            {
                yield return obj;
                obj = obj.nextObject;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override T Value<T>() => throw new NotImplementedException();
    }
}
