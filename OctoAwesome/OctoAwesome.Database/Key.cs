using System;
using System.Collections.Generic;

namespace OctoAwesome.Database
{
    public readonly struct Key<TTag> : IEquatable<Key<TTag>> where TTag : ITag, new()
    {
        private const int BASE_KEY_SIZE = sizeof(long) + sizeof(int);

        public static int KEY_SIZE { get; }

        public static Key<TTag> Empty { get; }

        static Key()
        {
            var emptyTag = new TTag();
            KEY_SIZE = emptyTag.Length + BASE_KEY_SIZE;
            Empty = new Key<TTag>();
        }

        public TTag Tag { get; }

        /// <summary>
        /// Current Position of the referenced Value in the ValueFile
        /// </summary>
        public long Index { get; }

        public int ValueLength { get; }

        /// <summary>
        /// Current Position of the Key in the KeyStore File
        /// </summary>
        public long Position { get; }

        public bool IsEmpty => Tag == null && ValueLength == 0;

        public Key(TTag tag, long index, int valueLength, long position)
        {
            Tag = tag;
            Index = index;
            ValueLength = valueLength;
            Position = position;
        }

        public Key(TTag tag, long index, int valueLength) : this(tag, index, valueLength, -1)
        {
        }

        public byte[] GetBytes()
        {
            var byteArray = new byte[KEY_SIZE];
            Buffer.BlockCopy(BitConverter.GetBytes(Index), 0, byteArray, 0, sizeof(long));
            Buffer.BlockCopy(BitConverter.GetBytes(ValueLength), 0, byteArray, sizeof(long), sizeof(int));

            if (Tag != null)
                Buffer.BlockCopy(Tag.GetBytes(), 0, byteArray, BASE_KEY_SIZE, Tag.Length);

            return byteArray;
        }

        public static Key<TTag> FromBytes(byte[] array, int index)
        {
            var localIndex = BitConverter.ToInt64(array, index);
            var length = BitConverter.ToInt32(array, index + sizeof(long));
            var tag = new TTag();
            tag.FromBytes(array, index + BASE_KEY_SIZE);

            return new Key<TTag>(tag, localIndex, length, index);
        }

        public override bool Equals(object obj) => obj is Key<TTag> key && Equals(key);

        public bool Equals(Key<TTag> other) => EqualityComparer<TTag>.Default.Equals(Tag, other.Tag) && ValueLength == other.ValueLength;

        public override int GetHashCode()
        {
            var hashCode = 139101280;
            hashCode = hashCode * -1521134295 + EqualityComparer<TTag>.Default.GetHashCode(Tag);
            hashCode = hashCode * -1521134295 + ValueLength.GetHashCode();
            return hashCode;
        }

        public bool Validate() => ValueLength >= 0 && Position >= 0 && Index >= 0 && KEY_SIZE > BASE_KEY_SIZE;

        public static bool operator ==(Key<TTag> left, Key<TTag> right) => left.Equals(right);

        public static bool operator !=(Key<TTag> left, Key<TTag> right) => !(left == right);
    }
}
