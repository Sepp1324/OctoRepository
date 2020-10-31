using System;
using System.Collections.Generic;

namespace OctoAwesome.Database
{
    internal class KeyStore<TTag> : IDisposable where TTag : ITag, new()
    {
        private readonly Dictionary<TTag, Key<TTag>> _keys;
        private readonly Writer _writer;
        private readonly Reader _reader;

        public KeyStore(Writer writer, Reader reader)
        {
            _keys = new Dictionary<TTag, Key<TTag>>();

            _writer = writer;
            _reader = reader;
        }

        public void Open()
        {
            _writer.Open();
            var buffer = _reader.Read(0, -1);

            for (var i = 0; i < buffer.Length; i += Key<TTag>.KEY_SIZE)
            {
                var key = Key<TTag>.FromBytes(buffer, i);
                _keys.Add(key.Tag, key);
            }
        }

        internal Key<TTag> GetKey(TTag tag) => _keys[tag];

        internal bool Contains(TTag tag) => _keys.ContainsKey(tag);

        internal void Add(Key<TTag> key)
        {
            _keys.Add(key.Tag, key);
            _writer.ToEnd();
            _writer.WriteAndFlush(key.GetBytes(), 0, Key<TTag>.KEY_SIZE);
        }

        public void Dispose() => _writer.Dispose();

        public void Remove(TTag tag, out Key<TTag> key)
        {
            key = _keys[tag];
            _keys.Remove(tag);
            _writer.ToEnd();
            _writer.WriteAndFlush(key.GetBytes(), 0, Key<TTag>.KEY_SIZE);
        }
    }
}