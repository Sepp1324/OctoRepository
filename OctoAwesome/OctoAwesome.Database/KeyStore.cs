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

        internal void Update(Key<TTag> key)
        {
            var oldKey = _keys[key.Tag];
            _keys[key.Tag] = key = new Key<TTag>(key.Tag, key.Index, key.Length, oldKey.Position);
            _writer.WriteAndFlush(key.GetBytes(), 0, Key<TTag>.KEY_SIZE, oldKey.Position);
        }

        internal void Add(Key<TTag> key)
        {
            key = new Key<TTag>(key.Tag, key.Index, key.Length, _writer.ToEnd());
            _keys.Add(key.Tag, key);
            _writer.WriteAndFlush(key.GetBytes(), 0, Key<TTag>.KEY_SIZE);
        }

        public void Dispose() => _writer.Dispose();

        public void Remove(TTag tag, out Key<TTag> key)
        {
            key = _keys[tag];
            _keys.Remove(tag);
            _writer.WriteAndFlush(Key<TTag>.Empty.GetBytes(), 0, Key<TTag>.KEY_SIZE, key.Position);
        }
    }
}