namespace OctoAwesome
{
    public interface IPlanetResourceManager
    {
        IChunk GetChunk(Index3 index);

        ushort GetBlock(Index3 index);

        ushort GetBlock(int x, int y, int z);

        void SetBlock(Index3 index, ushort block);

        void SetBlock(int x, int y, int z, ushort block);

        int GetBlockMeta(int x, int y, int z);

        void SetBlockMeta(int x, int y, int z, int meta);
    }
}