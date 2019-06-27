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
    }
}
