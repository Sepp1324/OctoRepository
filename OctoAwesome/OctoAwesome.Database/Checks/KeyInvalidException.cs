using System;
using System.Runtime.Serialization;

namespace OctoAwesome.Database.Checks
{
    [Serializable]
    public sealed class KeyInvalidException : Exception
    {
        private long Position { get; set; }

        public KeyInvalidException(string message, long position) : base($"{message} on Position {position}")
        {
            Position = position;
            Data.Add(nameof(Position), position);
        }

        private KeyInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
