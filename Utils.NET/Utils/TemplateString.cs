using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.Logging;

namespace Utils.NET.Utils
{
    public class TemplateString
    {
        private string[] bodyStrings;

        private string[] keys;

        public TemplateString(string template, char keyCharacter)
        {
            bodyStrings = template.Split(keyCharacter);
            keys = new string[bodyStrings.Length - 1];
            for (int i = 1; i < bodyStrings.Length; i++)
            {
                bodyStrings[i] = GetKeyAndTruncate(bodyStrings[i], out var key);
                keys[i - 1] = key;
            }
        }

        private string GetKeyAndTruncate(string input, out string word)
        {
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (!char.IsLetter(c))
                {
                    word = input.Substring(0, i);
                    return input.Substring(i);
                }
            }

            word = input;
            return "";
        }

        public string Build(Dictionary<string, string> keyValues)
        {
            var builder = new StringBuilder(bodyStrings[0]);
            for (int i = 1; i < bodyStrings.Length; i++)
            {
                var key = keys[i - 1];
                if (!keyValues.TryGetValue(key, out var value))
                    value = key;

                builder.Append(value);
                builder.Append(bodyStrings[i]);
            }
            return builder.ToString();
        }
    }
}
