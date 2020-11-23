using OctoAwesome.Noise;

namespace OctoAwesome.Basics.Biomes
{
    public interface IBiome
    {
        IPlanet Planet { get; }

        float MinValue { get; }

        float MaxValue { get; }

        float ValueRangeOffset { get; }

        float ValueRange { get; }

        INoise BiomeNoiseGenerator { get; }

        float[,] GetHeightmap(Index2 chunkIndex);
    }
}
