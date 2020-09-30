using System;
using System.Collections;
using System.Collections.Generic;

namespace OctoAwesome.Database
{
    public class Database : IDictionary<Key, Value>
    {
        private readonly KeyStore keyStore;
        private readonly ValueStore valueStore;

        public Database()
        {
            keyStore = new KeyStore();
            valueStore = new ValueStore();
        }

        public Value this[Key key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<Key> Keys => throw new NotImplementedException();

        public ICollection<Value> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int value, Value value)
        {
            if (keyStore.Contains(key))
                throw new ArgumentException($"{nameof(value)} already exists");

            var key = valueStore.AddValue(value, value);
            keyStore.Add(key);
        }

        public void Add(KeyValuePair<Key, Value> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<Key, Value> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(Key key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<Key, Value>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(Key key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<Key, Value> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(Key key, out Value value)
        {
            throw new NotImplementedException();
        }

        public void Update(object key, object value) => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
