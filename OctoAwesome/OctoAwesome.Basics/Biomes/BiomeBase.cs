using System.Buffers;
using System.Collections.Generic;
using OctoAwesome.Noise;

namespace OctoAwesome.Basics.Biomes
{
    /// <summary>
    /// Base-Class for all Biomes
    /// </summary>
    public abstract class BiomeBase : IBiome
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="planet">Current <see cref="Planet"/></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="valueRangeOffset"></param>
        /// <param name="valueRange"></param>
        protected BiomeBase(IPlanet planet, float minValue, float maxValue, float valueRangeOffset, float valueRange)
        {
            SubBiomes = new();
            Planet = planet;
            MinValue = minValue;
            MaxValue = maxValue;
            ValueRangeOffset = valueRangeOffset;
            ValueRange = valueRange;
        }

        /// <summary>
        /// List of SubBiomes
        /// </summary>
        public List<IBiome> SubBiomes { get; protected set; }

        /// <summary>
        /// Current Planet
        /// </summary>
        public IPlanet Planet { get; }

        /// <summary>
        /// Noise-Generator for specif Biome
        /// </summary>
        public INoise BiomeNoiseGenerator { get; protected set; }

        /// <summary>
        /// Min Value for Biome
        /// </summary>
        public float MinValue { get; protected set; }

        /// <summary>
        /// Max Value for Biome
        /// </summary>
        public float MaxValue { get; protected set; }

        /// <summary>
        /// Offset for Biome
        /// </summary>
        public float ValueRangeOffset { get; protected set; }

        /// <summary>
        /// Range for Biome
        /// </summary>
        public float ValueRange { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkIndex"></param>
        /// <param name="heightmap"></param>
        /// <returns></returns>
        public virtual float[] GetHeigthMap(Index2 chunkIndex, float[] heightmap)
        {
            chunkIndex = new(chunkIndex.X * Chunk.CHUNKSIZE_X, chunkIndex.Y * Chunk.CHUNKSIZE_Y);
            var heights = ArrayPool<float>.Shared.Rent(Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);
            for (var i = 0; i < heights.Length; i++)
                heights[i] = 0;
            BiomeNoiseGenerator.GetTileableNoiseMap2D(chunkIndex.X, chunkIndex.Y, Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y,
                Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y, heights);

            for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
            for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                heightmap[y * Chunk.CHUNKSIZE_X + x] = (heights[y * Chunk.CHUNKSIZE_X + x] / 2 + 0.5f) * ValueRange + ValueRangeOffset;
            ArrayPool<float>.Shared.Return(heights);
            return heightmap;
        }
    }
}