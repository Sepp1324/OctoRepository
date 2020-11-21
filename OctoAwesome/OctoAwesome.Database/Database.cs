using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Database
{
    public abstract class Database : IDisposable
    {
        public Type TagType { get; }

        protected Database(Type tagType) => TagType = tagType;

        public abstract void Open();

        public abstract void Dispose();
    }

    public sealed class Database<TTag> : Database where TTag : ITag, new()
    {
        public IEnumerable<TTag> Keys => _keyStore.Tags;

        private readonly KeyStore<TTag> _keyStore;
        private readonly ValueStore _valueStore;

        public Database(FileInfo keyFile, FileInfo valueFile) : base(typeof(TTag))
        {
            _keyStore = new KeyStore<TTag>(new Writer(keyFile), new Reader(keyFile));
            _valueStore = new ValueStore(new Writer(valueFile), new Reader(valueFile));
        }

        public override void Open()
        {
            _keyStore.Open();
            _valueStore.Open();
        }


        public Value GetValue(TTag tag)
        {
            var key = _keyStore.GetKey(tag);
            return _valueStore.GetValue(key);
        }

        public void AddOrUpdate(TTag tag, Value value)
        {
            var contains = _keyStore.Contains(tag);

            if (contains)
            {
                var key = _keyStore.GetKey(tag);
                _valueStore.Remove(key);
            }

            var newKey = _valueStore.AddValue(tag, value);

            if (contains)
                _keyStore.Update(newKey);
            else
                _keyStore.Add(newKey);
        }

        public bool ContainsKey(TTag tag) => _keyStore.Contains(tag);

        public void Remove(TTag tag)
        {
            _keyStore.Remove(tag, out var key);
            _valueStore.Remove(key);
        }

        public override void Dispose()
        {
            _keyStore.Dispose();
            _valueStore.Dispose();
        }
    }
}
