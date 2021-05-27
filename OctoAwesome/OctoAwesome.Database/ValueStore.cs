using System;

namespace OctoAwesome.Database
{
    internal class ValueStore : IDisposable
    {
        public bool FixedValueLength { get;  }

<<<<<<< HEAD
        private readonly Writer _writer;
        private readonly Reader _reader;

        public ValueStore(Writer writer, Reader reader, bool fixedValueLength)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _reader = reader ?? throw new ArgumentNullException(nameof(reader)); 
=======
        private readonly Writer writer;
        private readonly Reader reader;

        public ValueStore(Writer writer, Reader reader, bool fixedValueLength)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader)); 
>>>>>>> feature/performance
            FixedValueLength = fixedValueLength;
        }
        public ValueStore(Writer writer, Reader reader) : this(writer, reader, false)
        {

        }
        
        public Value GetValue<TTag>(Key<TTag> key) where TTag : ITag, new()
        {
            var byteArray = _reader.Read(key.Index + Key<TTag>.KEY_SIZE, key.ValueLength);
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

        /// <summary>
        /// Update a value on the exact <paramref name="key"/> index <see cref="Key{TTag}.Index"/> 
        /// </summary>
        /// <typeparam name="TTag"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException">If <see cref="FixedValueLength"/> is false</exception>
        internal void Update<TTag>(Key<TTag> key, Value value) where TTag : ITag, new()
        {
            if (!FixedValueLength)
                throw new NotSupportedException("Update is not allowed when value have no fixed size");

            _writer.WriteAndFlush(value.Content, 0, key.ValueLength, key.Index + Key<TTag>.KEY_SIZE);
        }

        internal void Remove<TTag>(Key<TTag> key) where TTag : ITag, new()
        {
<<<<<<< HEAD
            _writer.Write(Key<TTag>.Empty.GetBytes(), 0, Key<TTag>.KEY_SIZE, key.Index);
            _writer.WriteAndFlush(BitConverter.GetBytes(key.ValueLength), 0, sizeof(int), key.Index + Key<TTag>.KEY_SIZE);
=======
            writer.Write(Key<TTag>.Empty.GetBytes(), 0, Key<TTag>.KEY_SIZE, key.Index);
            writer.WriteAndFlush(BitConverter.GetBytes(key.ValueLength), 0, sizeof(int), key.Index + Key<TTag>.KEY_SIZE);
>>>>>>> feature/performance
        }

        internal void Open() => _writer.Open();

<<<<<<< HEAD
        internal void Close() => _writer.Close();

        public void Dispose() => _writer.Dispose();
=======
        internal void Close()
        {
            writer.Close();
        }

        public void Dispose()
        {
            writer.Dispose(); //TODO: Move to owner
        }
        
>>>>>>> feature/performance
    }
}
