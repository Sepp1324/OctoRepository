﻿using System;
using OctoAwesome.Database;

namespace OctoAwesome.Serialization
{
    public struct Index3Tag : ITag, IEquatable<Index3Tag>
    {
        public int Length => sizeof(int) * 3;

        public Index3 Index { get; private set; }

        public Index3Tag(Index3 index)
        {
            Index = index;
        }

        public void FromBytes(byte[] array, int startIndex)
        {
            Index = new Index3(BitConverter.ToInt32(array, startIndex),
                BitConverter.ToInt32(array, startIndex + sizeof(int)),
                BitConverter.ToInt32(array, startIndex + sizeof(int) + sizeof(int)));
        }

        public byte[] GetBytes()
        {
            var byteArray = new byte[Length];
            Buffer.BlockCopy(BitConverter.GetBytes(Index.X), 0, byteArray, 0, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(Index.Y), 0, byteArray, sizeof(int), sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(Index.Z), 0, byteArray, sizeof(int) + sizeof(int), sizeof(int));
            return byteArray;
        }

        public override bool Equals(object obj)
        {
            return obj is Index3Tag tag && Equals(tag);
        }

        public bool Equals(Index3Tag other)
        {
            return Length == other.Length && Index.Equals(other.Index);
        }

        public override int GetHashCode()
        {
            var hashCode = 802246856;
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + Index.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Index3Tag left, Index3Tag right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Index3Tag left, Index3Tag right)
        {
            return !(left == right);
        }
    }
}