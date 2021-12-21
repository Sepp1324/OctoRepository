using System;
using System.Buffers;
using OctoAwesome.Noise;

namespace OctoAwesome.Basics.Biomes
{
    public class SurfaceBiomeGenerator : LargeBiomeBase
    {
        public SurfaceBiomeGenerator(IPlanet planet, int seaLevel) : base(planet, 0f, 1f)
        {
            SeaLevel = seaLevel;
            BiomeNoiseGenerator = new SimplexNoiseGenerator(planet.Seed) { FrequencyX = 1f / 10000, FrequencyY = 1f / 10000, Factor = 1f };

            var offset = (float)seaLevel / (Planet.Size.Z * Chunk.CHUNKSIZE_Z);

            SubBiomes.Add(new OceanBiomeGenerator(planet, 0f, 0.3f, 0f, offset));
            SubBiomes.Add(new LandBiomeGenerator(planet, 0.5f, 1f, offset, 1 - offset));

            SortSubBiomes();
        }

        public int SeaLevel { get; }

        protected override float CurveFunction(float inputValue) => CurveFunction(inputValue, -0.08f, 200);

        private float CurveFunction(float inputValue, float brightness, int contrast)
        {
            inputValue += brightness;
            var factor = 259f / 255f * (contrast + 255) / (259 - contrast);
            inputValue = factor * (inputValue - 0.5f) + 0.5f;
            return Math.Min(Math.Max(inputValue, 0f), 1f);
        }

        public override float[] GetHeightmap(Index2 chunkIndex, float[] heightmap)
        {
            var blockIndex = new Index2(chunkIndex.X * Chunk.CHUNKSIZE_X, chunkIndex.Y * Chunk.CHUNKSIZE_Y);

            var regions = ArrayPool<float>.Shared.Rent(Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);
            for (var i = 0; i < regions.Length; i++)
                regions[i] = 0;
            BiomeNoiseGenerator.GetTileableNoiseMap2D(blockIndex.X, blockIndex.Y, Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y,
                Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y, regions);

            var biomeValues = ArrayPool<float>.Shared.Rent(SubBiomes.Count * Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y); //float[SubBiomes.COunt][,]

            var tempArray = ArrayPool<float>.Shared.Rent(Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);
            for (var i = 0; i < SubBiomes.Count; i++)
            {
                SubBiomes[i].GetHeightmap(chunkIndex, tempArray);
                Array.Copy(tempArray, 0, biomeValues, i * Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y, Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);
            }

            ArrayPool<float>.Shared.Return(tempArray);

            for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
            for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++)
            {
                var region = regions[y * Chunk.CHUNKSIZE_X + x] / 2 + 0.5f;

                var biome1 = ChooseBiome(region, out int biome2);

                var interpolationValue = 0f;
                if (biome2 != -1)
                {
                    interpolationValue = CalculateInterpolationValue(region, SubBiomes[biome1], SubBiomes[biome2]);
                    heightmap[y * Chunk.CHUNKSIZE_X + x] =
                        biomeValues[biome2 * Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y + y * Chunk.CHUNKSIZE_X + x] *
                        interpolationValue +
                        biomeValues[biome1 * Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y + y * Chunk.CHUNKSIZE_X + x] *
                        (1 - interpolationValue);
                }
                else
                {
                    heightmap[y * Chunk.CHUNKSIZE_X + x] =
                        biomeValues[biome1 * Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y + y * Chunk.CHUNKSIZE_X + x];
                }
            }

            ArrayPool<float>.Shared.Return(regions);
            ArrayPool<float>.Shared.Return(biomeValues);
            return heightmap;
        }
    }
}