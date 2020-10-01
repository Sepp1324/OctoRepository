using System;

namespace OctoAwesome.Database
{
    public class Value
    {
        public byte[] Content { get; }

        public Value(byte[] buffer)
        {
            Content = buffer;
        }
    }
}
