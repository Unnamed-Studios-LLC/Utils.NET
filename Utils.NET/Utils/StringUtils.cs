using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.NET.Utils
{
    public static class StringUtils
    {

        public static uint ParseHex(string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);
            return Convert.ToUInt32(hex, 16);
        }

        public static IEnumerable<T> ComponentsFromString<T>(string source, char delimeter, Func<string, T> parser)
        {
            return source.Split(delimeter).Select(_ => parser(_));
        }

        public static string ComponentsToString<T>(char delimeter, params T[] components)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < components.Length; i++)
            {
                builder.Append(components[i].ToString());
                if (i != components.Length - 1)
                    builder.Append(delimeter);
            }
            return builder.ToString();
        }

        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        public static string ApplyPlural(string word, int count)
        {
            if (count == 1)
                return word;
            return word + 's';
        }
    }
}
