using System;

namespace OctoAwesome
{
    public readonly struct BlockInfo : IEquatable<BlockInfo>
    {
        public static BlockInfo EMPTY = default;

        public bool IsEmpty => this == default;

        public Index3 Position { get; }

        public ushort Block { get; }

        public int Meta { get; }

        public BlockInfo(Index3 position, ushort block, int meta = 0)
        {
            Position = position;
            Block = block;
            Meta = meta;
        }

        public BlockInfo(int x, int y, int z, ushort block, int meta = 0) : this(new Index3(x, y, z), block, meta)
        {

        }

        public bool Equals(BlockInfo other) => Position.Equals(other.Position) && Block == other.Block && Meta == other.Meta;

        public override bool Equals(object obj) => obj is BlockInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Block.GetHashCode();
                hashCode = (hashCode * 397) ^ Meta;
                return hashCode;
            }
        }

        public static bool operator ==(BlockInfo left, BlockInfo right) => left.Equals(right);

        public static bool operator !=(BlockInfo left, BlockInfo right) => !(left == right);

        #region Blockinfo <=> Tuple
        public static implicit operator BlockInfo((int x, int y, int z, ushort block, int meta) blockTuple) => new BlockInfo(blockTuple.x, blockTuple.y, blockTuple.z, blockTuple.block, blockTuple.meta);

        public static implicit operator (int x, int y, int z, ushort block, int meta)(BlockInfo blockInfo) => (blockInfo.Position.X, blockInfo.Position.Y, blockInfo.Position.Z, blockInfo.Block, blockInfo.Meta);

        public static implicit operator BlockInfo((int x, int y, int z, ushort block) blockTuple) => new BlockInfo(blockTuple.x, blockTuple.y, blockTuple.z, blockTuple.block);

        public static implicit operator (int x, int y, int z, ushort block)(BlockInfo blockInfo) => (blockInfo.Position.X, blockInfo.Position.Y, blockInfo.Position.Z, blockInfo.Block);

        public static implicit operator BlockInfo((Index3 position, ushort block, int meta) blockTuple) => new BlockInfo(blockTuple.position, blockTuple.block, blockTuple.meta);

        public static implicit operator (Index3 position, ushort block, int meta)(BlockInfo blockInfo) => (blockInfo.Position, blockInfo.Block, blockInfo.Meta);

        public static implicit operator BlockInfo((Index3 position, ushort block) blockTuple) => new BlockInfo(blockTuple.position, blockTuple.block);

        public static implicit operator (Index3 position, ushort block)(BlockInfo blockInfo) => (blockInfo.Position, blockInfo.Block);
        #endregion
    }
}
