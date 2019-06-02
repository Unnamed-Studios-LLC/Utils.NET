using System;
using System.IO;
using System.Text;

namespace Utils.NET.IO.Tbon
{
    public static class Tbon
    {
        #region Deserialization

        /*public static T Deserialize<T>(StreamReader reader)
        {
            var type = typeof(T);
            T obj;
            if (type.IsClass)
            {
                obj = (T)Activator.CreateInstance(typeof(T));
            }
            else if (type.IsValueType && !type.IsEnum)
            {
                obj = default;
            }
        }*/

        public static TObject DynamicParse(string file)
        {
            return DynamicParse(File.OpenRead(file));
        }

        public static TObject DynamicParse(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return DynamicParse(reader);
            }
        }

        public static TObject DynamicParse(StreamReader reader)
        {
            return new TObject(reader);
        }

        #endregion

        #region Serialization



        #endregion
    }
}
