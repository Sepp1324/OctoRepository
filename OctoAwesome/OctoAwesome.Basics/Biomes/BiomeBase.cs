﻿using System.Buffers;
using System.Collections.Generic;
using OctoAwesome.Noise;

namespace OctoAwesome.Basics.Biomes
{
    public abstract class BiomeBase : IBiome
    {
        public BiomeBase(IPlanet planet, float minValue, float maxValue, float valueRangeOffset, float valueRange)
        {
            SubBiomes = new List<IBiome>();
            Planet = planet;
            MinValue = minValue;
            MaxValue = maxValue;
            ValueRangeOffset = valueRangeOffset;
            ValueRange = valueRange;
        }

        public List<IBiome> SubBiomes { get; protected set; }
        public IPlanet Planet { get; }

        public INoise BiomeNoiseGenerator { get; protected set; }

        public float MinValue { get; protected set; }

        public float MaxValue { get; protected set; }

        public float ValueRangeOffset { get; protected set; }

        public float ValueRange { get; protected set; }

        public virtual float[] GetHeightmap(Index2 chunkIndex, float[] heightmap)
        {
            chunkIndex = new Index2(chunkIndex.X * Chunk.CHUNKSIZE_X, chunkIndex.Y * Chunk.CHUNKSIZE_Y);
            var heights = ArrayPool<float>.Shared.Rent(Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);
            for (var i = 0; i < heights.Length; i++)
                heights[i] = 0;
            BiomeNoiseGenerator.GetTileableNoiseMap2D(chunkIndex.X, chunkIndex.Y, Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y,
                Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y, heights);

            for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
            for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                heightmap[y * Chunk.CHUNKSIZE_X + x] =
                    (heights[y * Chunk.CHUNKSIZE_X + x] / 2 + 0.5f) * ValueRange + ValueRangeOffset;
            ArrayPool<float>.Shared.Return(heights);
            return heightmap;
        }
    }
}