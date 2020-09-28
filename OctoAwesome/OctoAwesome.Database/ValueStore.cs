﻿using System.IO;

namespace OctoAwesome.Database
{
    internal class ValueStore
    {
        private readonly FileStream fileStream;

        public ValueStore(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        public bool TryGetValue(Key key, out Value value)
        {
            var buffer = new byte[key.Length];
            fileStream.Seek(key.Index, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, key.Length);
            value = new Value(buffer, key);
            return true;
        }

        public bool TryAddValue(Key key, Value value)
        {
            fileStream.Seek(0, SeekOrigin.End);
            fileStream.Write(value.ToArray(), 0, key.Length);
            return true;
        }
    }
}
