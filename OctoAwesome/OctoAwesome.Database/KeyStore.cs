using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Database
{
    internal class KeyStore
    {
        private readonly Dictionary<int, Key> keys;
        private readonly FileStream fileStream;

        public KeyStore(FileStream fileStream)
        {
            keys = new Dictionary<int, Key>();
            this.fileStream = fileStream;
        }

        public void Load()
        {
            var buffer = new byte[fileStream.Length];
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, buffer.Length);
        }

        internal bool Contains(int tag) => keys.ContainsKey(tag);

        internal void Add(Key key)
        {
            keys.Add(key.Tag, key);
            fileStream.Seek(0, SeekOrigin.End);
            fileStream.Write(key.GetBytes(), 0, Key.KEY_SIZE);
        }
    }
}
