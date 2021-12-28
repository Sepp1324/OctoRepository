using System;
using OctoAwesome.Noise;

namespace OctoAwesome.Basics.Climate
{
    public class ComplexClimateMap : IClimateMap
    {
        private readonly ComplexPlanet _planet;
        private readonly INoise _tempFluctuationGenerator;

        public ComplexClimateMap(ComplexPlanet planet)
        {
            _planet = planet;
            _tempFluctuationGenerator = new SimplexNoiseGenerator(planet.Seed - 1, 1f / 64, 1f / 64) { Octaves = 3 };
        }

        public IPlanet Planet => _planet;

        public float GetTemperature(Index3 blockIndex)
        {
            var equator = Planet.Size.Y * Chunk.CHUNKSIZE_Y / 2;
            var equatorTemperature = 40f;
            var poleTemperature = -20f;
            var tempFluctuation = _tempFluctuationGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y) * 5f;
            var temperatureDifference = poleTemperature - equatorTemperature;
            var temperatureDecreasePerBlock = 0.1f;
            float distance = Math.Abs(blockIndex.Y - equator);
            var temperature = tempFluctuation + equatorTemperature + temperatureDifference * (float)Math.Sin(Math.PI / 2 * distance / equator); //equatorTemperature + distance * temperatureDifference / equator;
            var height = (float)(blockIndex.Z - _planet.BiomeGenerator.SeaLevel) / (Planet.Size.Z * Chunk.CHUNKSIZE_Z - _planet.BiomeGenerator.SeaLevel);
            height = Math.Max(height, 0);
            height = height * height;
            return temperature - height * temperatureDecreasePerBlock;
        }

        public int GetPrecipitation(Index3 blockIndex)
        {
            var maxPrecipitation = 100;

            var rawValue = _planet.BiomeGenerator.BiomeNoiseGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y);

            var height = blockIndex.Z - _planet.BiomeGenerator.SeaLevel;
            float precipitationDecreasePerBlock = 1;

            return (int)((1 - rawValue) * maxPrecipitation - Math.Max(height, 0) * precipitationDecreasePerBlock);
        }
    }
}