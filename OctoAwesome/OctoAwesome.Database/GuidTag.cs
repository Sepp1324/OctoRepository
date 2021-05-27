using System;
using System.Linq;

namespace OctoAwesome.Database
{
    public struct GuidTag<T> : ITag, IEquatable<GuidTag<T>>
    {
        public Guid Tag { get; private set; }

        public int Length => 16;

        public GuidTag(Guid id)
        {
            Tag = id;
        }

        public byte[] GetBytes()
        {
            return Tag.ToByteArray();
        }

        public void FromBytes(byte[] array, int startIndex)
        {
            Tag = new Guid(array.Skip(startIndex).Take(Length).ToArray());
        }

        public override bool Equals(object obj)
        {
            return obj is GuidTag<T> tag && Equals(tag);
        }

        public bool Equals(GuidTag<T> other)
        {
            return Length == other.Length && Tag.Equals(other.Tag);
        }

        public override int GetHashCode()
        {
            var hashCode = 139101280;
            hashCode = hashCode * -1521134295 + Tag.GetHashCode();
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(GuidTag<T> left, GuidTag<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GuidTag<T> left, GuidTag<T> right)
        {
            return !(left == right);
        }
    }
}