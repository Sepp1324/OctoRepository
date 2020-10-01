using System;

namespace OctoAwesome.Database
{
    class Value
    {
        public Value(byte[] buffer, Key key)
        {
            Content = buffer;
            Key = key;
        }

        public Key Key { get; set; }

        public byte[] Content { get; set; }

        internal byte[] ToArray() => throw new NotImplementedException();
    }
}
