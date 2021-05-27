using System;
using System.Runtime.Serialization;

namespace OctoAwesome.Database.Checks
{
    [Serializable]
    public class KeyInvalidException : Exception
    {
        public KeyInvalidException(string message, long position) : base($"{message} on Position {position}")
        {
            Position = position;
            Data.Add(nameof(Position), position);
        }

        protected KeyInvalidException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public long Position { get; }
    }
}