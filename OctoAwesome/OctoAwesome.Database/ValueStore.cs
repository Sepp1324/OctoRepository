using System;

namespace OctoAwesome.Database
{
    internal class ValueStore : IDisposable
    {
        private readonly Writer _writer;
        private readonly Reader _reader;

        public ValueStore(Writer writer, Reader reader)
        {
            _writer = writer;
            _reader = reader;
        }

        public Value GetValue<TTag>(Key<TTag> key) where TTag : ITag, new()
        {
            var byteArray = _reader.Read(key.Index + Key<TTag>.KEY_SIZE, key.Length);
            return new Value(byteArray);
        }

        internal Key<TTag> AddValue<TTag>(TTag tag, Value value) where TTag : ITag, new()
        {
            var key = new Key<TTag>(tag, _writer.ToEnd(), value.Content.Length);
            //TODO: Hash, Sync
            _writer.Write(key.GetBytes(), 0, Key<TTag>.KEY_SIZE);
            _writer.WriteAndFlush(value.Content, 0, value.Content.Length);
            return key;
        }

        internal void Open() => _writer.Open();

        public void Dispose() => _writer.Dispose(); 
        public void Remove<TTag>(in Key<TTag> key) where TTag : ITag, new()
        {
            _writer.Write(Key<TTag>.Empty.GetBytes(), 0, Key<TTag>.KEY_SIZE, key.Index);
            _writer.WriteAndFlush(BitConverter.GetBytes(key.Length), 0, sizeof(int), key.Index + Key<TTag>.KEY_SIZE);
        }
    }
}