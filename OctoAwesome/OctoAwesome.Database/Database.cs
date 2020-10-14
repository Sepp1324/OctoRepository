using System;
using System.IO;

namespace OctoAwesome.Database
{
    public class Database<TTag> : IDisposable where TTag : ITagable
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
            if (_keyStore.Contains(tag))
                ;
            //TODO: Update
            else
                _keyStore.Add(_valueStore.AddValue(tag, value));
        }

        public void Remove(TTag tag)
        {
        }

        public void Dispose()
        {
            _keyStore.Dispose();
            _valueStore.Dispose();
        }
    }
}