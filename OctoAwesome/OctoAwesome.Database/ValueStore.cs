using System;

namespace OctoAwesome.Database
{
    internal class ValueStore : IDisposable
    {
        private readonly Reader _reader;

        private readonly Writer _writer;

        public ValueStore(Writer writer, Reader reader, bool fixedValueLength)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            FixedValueLength = fixedValueLength;
        }

        public ValueStore(Writer writer, Reader reader) : this(writer, reader, false)
        {
        }

        public bool FixedValueLength { get; }

        public void Dispose() => _writer.Dispose(); //TODO: Move to owner

        public Value GetValue<TTag>(Key<TTag> key) where TTag : ITag, new()
        {
            var byteArray = _reader.Read(key.Index + Key<TTag>.KEY_SIZE, key.ValueLength);
            return new(byteArray);
        }

        internal Key<TTag> AddValue<TTag>(TTag tag, Value value) where TTag : ITag, new()
        {
            var key = new Key<TTag>(tag, _writer.ToEnd(), value.Content.Length);
            //TODO: Hash, Sync
            key.WriteBytes(_writer);
            _writer.WriteAndFlush(value.Content, 0, value.Content.Length);
            return key;
        }

        /// <summary>
        ///     Update a value on the exact <paramref name="key" /> index <see cref="Key{TTag}.Index" />
        /// </summary>
        /// <typeparam name="TTag"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException">If <see cref="FixedValueLength" /> is false</exception>
        internal void Update<TTag>(Key<TTag> key, Value value) where TTag : ITag, new()
        {
            if (!FixedValueLength)
                throw new NotSupportedException("Update is not allowed when value have no fixed size");

            _writer.WriteAndFlush(value.Content, 0, key.ValueLength, key.Index + Key<TTag>.KEY_SIZE);
        }

        internal void Remove<TTag>(Key<TTag> key) where TTag : ITag, new()
        {
            Key<TTag>.Empty.WriteBytes(_writer, key.Index);
            _writer.WriteAndFlush(BitConverter.GetBytes(key.ValueLength), 0, sizeof(int), key.Index + Key<TTag>.KEY_SIZE);
        }

        internal void Open() => _writer.Open();

        internal void Close() => _writer.Close();
    }
}