using System;

namespace OctoAwesome.Database
{
    class Key
    {
        public const int KEY_SIZE = sizeof(long) + sizeof(int) + sizeof(int);

        public int Target { get; set; }

        public long Index { get; set; }

        public int Length { get; set; }

        public Key(int target, long index, int length)
        {
            Target = target;
            Index = index;
            Length = length;
        }

        internal byte[] GetBytes()
        {
            var byteArray = new byte[KEY_SIZE];

            Buffer.BlockCopy(BitConverter.GetBytes(Target), 0, byteArray, 0, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(Index), 0, byteArray, sizeof(int), sizeof(long));
            Buffer.BlockCopy(BitConverter.GetBytes(Length), 0, byteArray, sizeof(int) + sizeof(long), sizeof(int));
            return byteArray;
        }
    }
}
