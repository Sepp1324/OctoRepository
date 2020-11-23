using OctoAwesome.Noise;
using System;

namespace OctoAwesome.Basics.Climate
{
    public class ComplexClimateMap : IClimateMap
    {
        public IPlanet Planet => _planet;

        ComplexPlanet _planet;
        private INoise _tempFluctuationGenerator;

        public ComplexClimateMap(ComplexPlanet planet)
        {
            _planet = planet;
            _tempFluctuationGenerator = new SimplexNoiseGenerator(planet.Seed - 1, 1f / 64, 1f / 64) { Octaves = 3};
        }

        public float GetTemperature(Index3 blockIndex)
        {
            int equator = (Planet.Size.Y * Chunk.CHUNKSIZE_Y) / 2;
            float equatorTemperature = 40f;
            float poleTemperature = -20f;
            float tempFluctuation = _tempFluctuationGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y) * 5f;
            float temperatureDifference = poleTemperature - equatorTemperature;
            float temperatureDecreasePerBlock = 0.1f;
            float distance = Math.Abs(blockIndex.Y - equator);
            float temperature = tempFluctuation + equatorTemperature + temperatureDifference * (float)Math.Sin((Math.PI / 2) * distance / equator);  //equatorTemperature + distance * temperatureDifference / equator;
            float height = (float)(blockIndex.Z - _planet.BiomeGenerator.SeaLevel) / (Planet.Size.Z * Chunk.CHUNKSIZE_Z - _planet.BiomeGenerator.SeaLevel);
            height = Math.Max(height, 0);
            height = height*height;
            return temperature - height * temperatureDecreasePerBlock;
        }

        public int GetPrecipitation(Index3 blockIndex)
        {
            int maxPrecipitation = 100;

            float rawValue = _planet.BiomeGenerator.BiomeNoiseGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y);

            int height = blockIndex.Z - _planet.BiomeGenerator.SeaLevel;
            float precipitationDecreasePerBlock = 1;

            return (int)(((1 - rawValue) * maxPrecipitation) - (Math.Max(height, 0) * precipitationDecreasePerBlock));
        }
    }
}
