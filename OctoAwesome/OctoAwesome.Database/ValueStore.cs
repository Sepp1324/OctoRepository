﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OctoAwesome.Database
{
    internal class ValueStore : IDisposable
    {
        private readonly Writer writer;
        private readonly Reader reader;

        public ValueStore(Writer writer, Reader reader)
        {
            this.writer = writer;
            this.reader = reader;
        }

        public Value GetValue(Key key)
        {
            var byteArray = reader.Read(key.Index + Key.KEY_SIZE, key.Length);
            return new Value(byteArray);
        }

        internal Key AddValue<TTag>(TTag tag, Value value) where TTag : ITagable
        {
            var key = new Key(tag.Tag, writer.ToEnd(), value.Content.Length);
            //TODO: Hash, Sync
            writer.Write(key.GetBytes(), 0, Key.KEY_SIZE);
            writer.WriteAndFlush(value.Content, 0, value.Content.Length);
            return key;
        }

        internal void Open()
        {
            writer.Open();
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}