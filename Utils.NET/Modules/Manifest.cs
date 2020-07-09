using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Utils.NET.Logging;

namespace Utils.NET.Modules
{
    public class Manifest
    {
        public static Manifest Load(string directory = null)
        {
            foreach (var file in Directory.EnumerateFiles(directory ?? Directory.GetCurrentDirectory(), "*.mfst", SearchOption.TopDirectoryOnly))
            {
                return new Manifest(file);
            }
            return new Manifest();
        }

        private JObject json;

        private string fileName;

        public Manifest()
        {

        }

        public Manifest(byte[] file)
        {
            using (var reader = new StreamReader(new MemoryStream(file)))
            {
                json = JObject.Parse(reader.ReadToEnd());
            }
        }

        public Manifest(string fileName)
        {
            this.fileName = fileName;
            json = JObject.Parse(File.ReadAllText(fileName));
        }

        public T Value<T>(string key, T defaultValue)
        {
            if (json == null) return defaultValue;

            var value = json.GetValue(key);
            if (value == null) return defaultValue;
            return value.Value<T>();
        }
    }
}
