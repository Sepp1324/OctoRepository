using OctoAwesome.Database;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Serialization
{
    public struct ChunkDiffTag : ITag, IEquatable<ChunkDiffTag>
    {
        public int Length => sizeof(int) * 4;

        public Index3 ChunkPositon { get; set; }
        public int FlatIndex { get; set; }

        public ChunkDiffTag(Index3 chunkPosition, int flatIndex)
        {
            ChunkPositon = chunkPosition;
            FlatIndex = flatIndex;
        }

        public bool Equals(ChunkDiffTag other) => Length == other.Length && EqualityComparer<Index3>.Default.Equals(ChunkPositon, other.ChunkPositon) && FlatIndex == other.FlatIndex;

        public override bool Equals(object obj) => obj is ChunkDiffTag other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (ChunkPositon.GetHashCode() * 397) ^ FlatIndex;
            }
        }

        public static bool operator ==(ChunkDiffTag left, ChunkDiffTag right) => left.Equals(right);

        public static bool operator !=(ChunkDiffTag left, ChunkDiffTag right) => !left.Equals(right);

        public void FromBytes(byte[] array, int startIndex)
        {
            var x = BitConverter.ToInt32(array, startIndex);
            var y = BitConverter.ToInt32(array, startIndex + sizeof(int));
            var z = BitConverter.ToInt32(array, startIndex + sizeof(int) * 2);
            FlatIndex = BitConverter.ToInt32(array, startIndex + sizeof(int) * 3);
            ChunkPositon = new Index3(x, y, z);
        }

        public byte[] GetBytes()
        {
            var array = new byte[Length];

            Buffer.BlockCopy(BitConverter.GetBytes(ChunkPositon.X), 0, array, 0, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(ChunkPositon.Y), 0, array, sizeof(int), sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(ChunkPositon.Z), 0, array, sizeof(int) * 2, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(FlatIndex), 0, array, sizeof(int) * 3, sizeof(int));

            return array;
        }
    }
}