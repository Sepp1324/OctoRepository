using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Database
{
    internal class KeyStore
    {
        private readonly Dictionary<object, Key> keys;

        public KeyStore()
        {
            keys = new Dictionary<object, Key>();
        }

        public void Load(FileStream fileStream)
        {
            var buffer = new byte[fileStream.Length];
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, buffer.Length);
        }

        internal bool Contains(object key)
        {
            throw new NotImplementedException();
        }

        internal void Add(object key)
        {
            throw new NotImplementedException();
        }
    }
}
