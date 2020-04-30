namespace OctoAwesome.Runtime
{
    public interface IPlanetResourceManager
    {
        IChunk GetChunk(Index3 index);

        IBlock GetBlock(Index3 index);

        IBlock GetBlock(int x, int y, int z);

        void SetBlock(Index3 index, IBlock block);

        void SetBlock(int x, int y, int z, IBlock block);
    }
}