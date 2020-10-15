using System;
using System.Collections.Generic;

namespace OctoAwesome.Database
{
    internal class KeyStore<TTag> : IDisposable where TTag : ITagable
    {
        private readonly Dictionary<int, Key> _keys;
        private readonly Writer _writer;
        private readonly Reader _reader;

        public KeyStore(Writer writer, Reader reader)
        {
            _keys = new Dictionary<int, Key>();

            _writer = writer;
            _reader = reader;
        }

        public void Open()
        {
            _writer.Open();
            var buffer = _reader.Read(0, -1);

            for (var i = 0; i < buffer.Length; i += Key.KEY_SIZE)
            {
                var key = Key.FromBytes(buffer, i);
                _keys.Add(key.Tag, key);
            }
        }

        internal Key GetKey(TTag tag) => _keys[tag.Tag];

        internal bool Contains(TTag tag)
        {
            return _keys.ContainsKey(tag.Tag);
        }

        internal void Add(Key key)
        {
            _keys.Add(key.Tag, key);
            _writer.ToEnd();
            _writer.WriteAndFlush(key.GetBytes(), 0, Key.KEY_SIZE);
        }

        public void Dispose() => _writer.Dispose();

        public void Remove(TTag tag, out Key key)
        {
            key = _keys[tag.Tag];
            _keys.Remove(tag.Tag);
            _writer.ToEnd();
            _writer.WriteAndFlush(key.GetBytes(), 0, Key.KEY_SIZE); //CONTINUE: 
        }
    }
}