using OctoAwesome.Noise;

namespace OctoAwesome.Basics.Biomes
{
    public class LandBiomeGenerator : LargeBiomeBase
    {
        public LandBiomeGenerator(IPlanet planet, float minVal, float maxVal, float valueRangeOffset, float valueRange)
            : base(planet, valueRangeOffset, valueRange)
        {
            BiomeNoiseGenerator = new SimplexNoiseGenerator(planet.Seed + 1) { FrequencyX = 1f / 1000, FrequencyY = 1f / 1000, Persistance = 0.25f, Octaves = 5, Factor = 1f };

            MinValue = minVal;
            MaxValue = maxVal;

            SubBiomes.Add(new FlatlandBiome(planet, 0f, 0.2f, 0f, 0.1f));
            SubBiomes.Add(new HillsBiome(planet, 0.3f, 0.5f, 0.1f, 0.4f));
            SubBiomes.Add(new HighMountainBiome(planet, 0.8f, 1f, 0.2f, 0.8f));

            SortSubBiomes();
        }

        public override float[,] GetHeightmap(Index2 chunkIndex)
        {
            var values = new float[Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y];
            var blockIndex = new Index2(chunkIndex.X * Chunk.CHUNKSIZE_X, chunkIndex.Y * Chunk.CHUNKSIZE_Y);
            var regions = BiomeNoiseGenerator.GetTileableNoiseMap2D(blockIndex.X, blockIndex.Y, Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y);
            var biomeValues = new float[SubBiomes.Count][,];

            for (var i = 0; i < SubBiomes.Count; i++)
                biomeValues[i] = SubBiomes[i].GetHeightmap(chunkIndex);

            for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
            {
                for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    var region = regions[x, y] / 2 + 0.5f;
                    var biome1 = ChooseBiome(region, out int biome2);
                    var interpolationValue = 0f;

                    if (biome2 != -1)
                    {
                        interpolationValue = CalculateInterpolationValue(region, SubBiomes[biome1], SubBiomes[biome2]);
                        values[x, y] = (biomeValues[biome2][x, y] * interpolationValue) + (biomeValues[biome1][x, y] * (1 - interpolationValue));
                    }
                    else
                        values[x, y] = biomeValues[biome1][x, y];
                }
            }
            return values;
        }
    }
}

