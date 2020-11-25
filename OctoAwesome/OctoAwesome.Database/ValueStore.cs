﻿using System;

namespace OctoAwesome.Database
{
    internal class ValueStore : IDisposable
    {
        private bool FixedValueLength { get;  }

        private readonly Writer writer;
        private readonly Reader reader;

        public ValueStore(Writer writer, Reader reader, bool fixedValueLength)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader)); 
            FixedValueLength = fixedValueLength;
        }
       
        public ValueStore(Writer writer, Reader reader) : this(writer, reader, false)
        {

        }
        
        /// <summary>
        /// Returns a Value
        /// </summary>
        /// <typeparam name="TTag"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Value GetValue<TTag>(Key<TTag> key) where TTag : ITag, new()
        {
            var byteArray = reader.Read(key.Index + Key<TTag>.KEY_SIZE, key.Length);
            return new Value(byteArray);
        }

        /// <summary>
        /// Adds a Value
        /// </summary>
        /// <typeparam name="TTag"></typeparam>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal Key<TTag> AddValue<TTag>(TTag tag, Value value) where TTag : ITag, new()
        {
            var key = new Key<TTag>(tag, writer.ToEnd(), value.Content.Length);
            //TODO: Hash, Sync
            writer.Write(key.GetBytes(), 0, Key<TTag>.KEY_SIZE);
            writer.WriteAndFlush(value.Content, 0, value.Content.Length);
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
                throw new NotSupportedException("Update is not allowed when the value have no fixed size");

            writer.WriteAndFlush(value.Content, 0, key.Length, key.Index + Key<TTag>.KEY_SIZE);
        }

        /// <summary>
        /// Removes a Value
        /// </summary>
        /// <typeparam name="TTag"></typeparam>
        /// <param name="key"></param>
        internal void Remove<TTag>(Key<TTag> key) where TTag : ITag, new()
        {
            writer.Write(Key<TTag>.Empty.GetBytes(), 0, Key<TTag>.KEY_SIZE, key.Index);
            writer.WriteAndFlush(BitConverter.GetBytes(key.Length), 0, sizeof(int), key.Index + Key<TTag>.KEY_SIZE);
        }

        /// <summary>
        /// Opens a Value Store
        /// </summary>
        internal void Open() => writer.Open();

        /// <summary>
        /// Disposes the Values
        /// </summary>
        public void Dispose() => writer.Dispose();
    }
}
