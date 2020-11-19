using System;
using System.IO;

namespace OctoAwesome.Database
{
    public class Database<TTag> : IDisposable where TTag : ITag, new()
    {
        private readonly KeyStore<TTag> _keyStore;
        private readonly ValueStore _valueStore;

        public Database(FileInfo keyFile, FileInfo valueFile)
        {
            _keyStore = new KeyStore<TTag>(new Writer(keyFile), new Reader(keyFile));
            _valueStore = new ValueStore(new Writer(valueFile), new Reader(valueFile));
        }

        public void Open()
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

        public void Dispose()
        {
            _keyStore.Dispose();
            _valueStore.Dispose();
        }
    }
}