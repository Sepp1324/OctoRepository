using System;
using System.Runtime.Serialization;

namespace OctoAwesome.Database.Checks
{
    [Serializable]
    public class CheckFailedException : Exception
    {
        private long position;

        public CheckFailedException()
        {
            
        }

        public CheckFailedException(string message) : base(message)
        {
            
        }

        public CheckFailedException(string message, Exception inner) : base(message, inner)
        {
            
        }

        protected CheckFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
        }

        public CheckFailedException(string message, long position) : this(message) =>
            this.position = position;
    }

    
}
