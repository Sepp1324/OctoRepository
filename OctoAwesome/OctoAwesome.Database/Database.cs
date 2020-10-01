using System;
using System.Collections;
using System.Collections.Generic;

namespace OctoAwesome.Database
{
    public class Database
    {
        private readonly KeyStore keyStore;
        private readonly ValueStore valueStore;

        public Database()
        {
            
        }

        public void Add(int tag, Value value)
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

        public void Get()
        {

        }
    }
}
