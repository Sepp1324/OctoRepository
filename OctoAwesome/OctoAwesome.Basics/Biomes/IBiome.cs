using OctoAwesome.Noise;

namespace OctoAwesome.Basics.Biomes
{
    /// <summary>
    /// Interface for Biomes
    /// </summary>
    public interface IBiome
    {
        /// <summary>
        /// Current Planet
        /// </summary>
        IPlanet Planet { get; }

        /// <summary>
        /// Min Value for Biome
        /// </summary>
        float MinValue { get; }

        /// <summary>
        /// Max Value for Biome
        /// </summary>
        float MaxValue { get; }

        /// <summary>
        /// RangeOffset for Biome
        /// </summary>
        float ValueRangeOffset { get; }

        /// <summary>
        /// Range for Biome
        /// </summary>
        float ValueRange { get; }

        /// <summary>
        /// Noise Generator for Biome
        /// </summary>
        INoise BiomeNoiseGenerator { get; }

        float[] GetHeigthMap(Index2 chunkIndex, float[] heightmap);
    }
}