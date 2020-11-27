using System;
using System.Collections.Generic;
using OctoAwesome.Database.Checks;

namespace OctoAwesome.Database
{
    internal class KeyStore<TTag> : IDisposable where TTag : ITag, new()
    {
        public int EmptyKeys { get; private set; }

        public IEnumerable<TTag> Tags => _keys.Keys;

        public IEnumerable<Key<TTag>> Keys => _keys.Values;

        private readonly Dictionary<TTag, Key<TTag>> _keys;
        private readonly Writer _writer;
        private readonly Reader _reader;

        public KeyStore(Writer writer, Reader reader)
        {
            _keys = new Dictionary<TTag, Key<TTag>>();

            this._writer = writer;
            this._reader = reader;
        }

        public void Open()
        {
            _keys.Clear();
            EmptyKeys = 0;

            _writer.Open();
            var buffer = _reader.Read(0, -1);

            for (var i = 0; i < buffer.Length; i += Key<TTag>.KEY_SIZE)
            {
                var key = Key<TTag>.FromBytes(buffer, i);

                if (!key.Validate())
                    throw new KeyInvalidException("Key is not valid", i);

                if (key.IsEmpty)
                {
                    EmptyKeys++;
                    continue;
                }

                _keys.Add(key.Tag, key);
            }
        }

        public void Close()
        {
            _writer.Close();
        }

        internal Key<TTag> GetKey(TTag tag) => _keys[tag];

        internal void Update(Key<TTag> key)
        {
            var oldKey = _keys[key.Tag];
            _keys[key.Tag] = new Key<TTag>(key.Tag, key.Index, key.ValueLength, oldKey.Position);
            _writer.WriteAndFlush(key.GetBytes(), 0, Key<TTag>.KEY_SIZE, oldKey.Position);
        }

        internal bool Contains(TTag tag) => _keys.ContainsKey(tag);

        internal void Add(Key<TTag> key)
        {
            key = new Key<TTag>(key.Tag, key.Index, key.ValueLength, _writer.ToEnd());
            _keys.Add(key.Tag, key);
            _writer.WriteAndFlush(key.GetBytes(), 0, Key<TTag>.KEY_SIZE);
        }

        internal void Remove(TTag tag, out Key<TTag> key)
        {
            key = _keys[tag];
            _keys.Remove(tag);
            _writer.WriteAndFlush(Key<TTag>.Empty.GetBytes(), 0, Key<TTag>.KEY_SIZE, key.Position);
        }

        public void Dispose()
        {
            _keys.Clear();
            _writer.Dispose();
        }
    }
}
