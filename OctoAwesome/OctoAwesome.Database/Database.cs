﻿using System;

namespace OctoAwesome.Database
{
    public class Database<TTag> where TTag : ITagable
    {
        private readonly KeyStore<TTag> keyStore;
        private readonly ValueStore valueStore;

        public Database()
        {
            
        }

        public Value GetValue(TTag tag)
        {
            var key = keyStore.GetKey(tag);
            return valueStore.GetValue(key);
        }

        public void Add(TTag tag, Value value)
        {
            if (keyStore.Contains(tag))
                throw new ArgumentException($"{nameof(value)} already exists");

            var key = valueStore.AddValue(tag, value);
            keyStore.Add(key);
        }

        public void Update()
        {

        }

        public void Remove()
        {

        }
    }
}
