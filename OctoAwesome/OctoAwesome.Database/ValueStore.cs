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

        public Value GetValue(Key key)
        {
            var byteArray = _reader.Read(key.Index + Key.KEY_SIZE, key.Length);
            return new Value(byteArray);
        }

        internal Key AddValue<TTag>(TTag tag, Value value) where TTag : ITagable
        {
            var key = new Key(tag.Tag, _writer.ToEnd(), value.Content.Length);
            //TODO: Hash, Sync
            _writer.Write(key.GetBytes(), 0, Key.KEY_SIZE);
            _writer.WriteAndFlush(value.Content, 0, value.Content.Length);
            return key;
        }

        internal void Open() => _writer.Open();

        public void Dispose() => _writer.Dispose();

        public void Remove(in Key key)
        {
            _writer.Write(Key.Empty.GetBytes(), 0, Key.KEY_SIZE, key.Index);
            _writer.WriteAndFlush(BitConverter.GetBytes(key.Length), 0, sizeof(int), key.Index + Key.KEY_SIZE);
        }
    }
}