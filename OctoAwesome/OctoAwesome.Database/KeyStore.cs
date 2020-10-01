﻿using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Database
{
    internal class KeyStore<TTag> where TTag : ITagable
    {
        private readonly Dictionary<int, Key> keys;
        private readonly FileStream fileStream;

        public KeyStore(FileStream fileStream)
        {
            keys = new Dictionary<int, Key>();
            this.fileStream = fileStream;
        }

        internal Key GetKey(TTag tag) => keys[tag.Tag];

        public void Load()
        {
            var buffer = new byte[fileStream.Length];
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, buffer.Length);
        }

        internal bool Contains(TTag tag) => keys.ContainsKey(tag.Tag);

        internal void Add(Key key)
        {
            keys.Add(key.Tag, key);
            fileStream.Seek(0, SeekOrigin.End);
            fileStream.Write(key.GetBytes(), 0, Key.KEY_SIZE);
        }
    }
}
