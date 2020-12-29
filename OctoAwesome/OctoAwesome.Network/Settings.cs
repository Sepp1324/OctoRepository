using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Network
{
    public class Settings : ISettings
    {
        private readonly Dictionary<string, string> _dictionary;

        public Settings(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
            _dictionary = InternalLoad(fileInfo);
        }

        public Settings()
        {
            _dictionary = new Dictionary<string, string>()
            {
                ["ChunkRoot"] = "ServerMap",
                ["Viewrange"] = "4",
                ["DisablePersistence"] = "false",
                ["LastUniverse"] = ""
            };
        }

        public FileInfo FileInfo { get; set; }

        public void Delete(string key) => _dictionary.Remove(key);

        public T Get<T>(string key) => (T) Convert.ChangeType(_dictionary[key], typeof(T));

        public T Get<T>(string key, T defaultValue)
        {
            if (_dictionary.TryGetValue(key, out var value))
                return (T) Convert.ChangeType(value, typeof(T));

            return defaultValue;
        }

        public T[] GetArray<T>(string key) => DeserializeArray<T>(_dictionary[key]);

        public bool KeyExists(string key) => _dictionary.ContainsKey(key);

        public void Set(string key, string value)
        {
            if (_dictionary.ContainsKey(key))
                _dictionary[key] = value;
            else
                _dictionary.Add(key, value);

            Save();
        }

        public void Set(string key, int value) => Set(key, value.ToString());

        public void Set(string key, bool value) => Set(key, value.ToString());

        public void Set(string key, string[] values) => Set(key, "[" + string.Join(",", values) + "]");

        public void Set(string key, int[] values) => Set(key, values.Select(i => i.ToString()).ToArray());

        public void Set(string key, bool[] values) => Set(key, values.Select(b => b.ToString()).ToArray());

        public void Load()
        {
            _dictionary.Clear();

            foreach (var entry in InternalLoad(FileInfo))
                _dictionary.Add(entry.Key, entry.Value);
        }


        public void Save()
        {
            FileInfo.Delete();
            using (var writer = new StreamWriter(FileInfo.OpenWrite()))
            {
                writer.Write(JsonConvert.SerializeObject(_dictionary, Formatting.Indented));
            }
        }

        private Dictionary<string, string> InternalLoad(FileInfo fileInfo)
        {
            using (var reader = new StreamReader(fileInfo.OpenRead()))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
            }
        }

        private T[] DeserializeArray<T>(string arrayString)
        {
            // Wir müssten, um beide Klammern zu entfernen, - 3 rechnen. Ich lasse die letzte Klammer stellvertretend für das Komma, was folgen würde, stehen.
            // Das wird in der for-Schleife auseinander gepflückt.

            arrayString = arrayString.Substring(1, arrayString.Length - 2 /*- 1*/);

            var partsString = arrayString.Split(',');
            var tArray = new T[partsString.Length];
            for (var i = 0; i < partsString.Length; i++)
                tArray[i] = (T) Convert.ChangeType(partsString[i], typeof(T));

            return tArray;
        }
    }
}