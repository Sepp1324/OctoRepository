using OctoAwesome.Noise;
using System;

namespace OctoAwesome.Basics.Climate
{
    public class ComplexClimateMap : IClimateMap
    {
<<<<<<< HEAD
        private readonly ComplexPlanet _planet;
        private readonly INoise _tempFluctuationGenerator;
        
        public IPlanet Planet => _planet;

        public ComplexClimateMap(ComplexPlanet planet)
        {
            _planet = planet;
            _tempFluctuationGenerator = new SimplexNoiseGenerator(planet.Seed - 1, 1f / 64, 1f / 64) { Octaves = 3};
=======
        public IPlanet Planet => planet;

        ComplexPlanet planet;
        private INoise tempFluctuationGenerator;

        public ComplexClimateMap(ComplexPlanet planet)
        {
            this.planet = planet;
            tempFluctuationGenerator = new SimplexNoiseGenerator(planet.Seed - 1, 1f / 64, 1f / 64) { Octaves = 3};
>>>>>>> feature/performance
        }

        public float GetTemperature(Index3 blockIndex)
        {
<<<<<<< HEAD
            var equator = (Planet.Size.Y * Chunk.CHUNKSIZE_Y) / 2;
            var equatorTemperature = 40f;
            var poleTemperature = -20f;
            var tempFluctuation = _tempFluctuationGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y) * 5f;
            var temperatureDifference = poleTemperature - equatorTemperature;
            var temperatureDecreasePerBlock = 0.1f;
            var distance = Math.Abs(blockIndex.Y - equator);
            var temperature = tempFluctuation + equatorTemperature + temperatureDifference * (float)Math.Sin((Math.PI / 2) * distance / equator);  //equatorTemperature + distance * temperatureDifference / equator;
            var height = (float)(blockIndex.Z - _planet.BiomeGenerator.SeaLevel) / (Planet.Size.Z * Chunk.CHUNKSIZE_Z - _planet.BiomeGenerator.SeaLevel);
=======
            int equator = (Planet.Size.Y * Chunk.CHUNKSIZE_Y) / 2;
            float equatorTemperature = 40f;
            float poleTemperature = -20f;
            float tempFluctuation = tempFluctuationGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y) * 5f;
            float temperatureDifference = poleTemperature - equatorTemperature;
            float temperatureDecreasePerBlock = 0.1f;
            float distance = Math.Abs(blockIndex.Y - equator);
            float temperature = tempFluctuation + equatorTemperature + temperatureDifference * (float)Math.Sin((Math.PI / 2) * distance / equator);  //equatorTemperature + distance * temperatureDifference / equator;
            float height = (float)(blockIndex.Z - planet.BiomeGenerator.SeaLevel) / (Planet.Size.Z * Chunk.CHUNKSIZE_Z - planet.BiomeGenerator.SeaLevel);
>>>>>>> feature/performance
            height = Math.Max(height, 0);
            height = height*height;
            return temperature - height * temperatureDecreasePerBlock;
        }

        public int GetPrecipitation(Index3 blockIndex)
        {
<<<<<<< HEAD
            var maxPrecipitation = 100;
            var rawValue = _planet.BiomeGenerator.BiomeNoiseGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y);
            var height = blockIndex.Z - _planet.BiomeGenerator.SeaLevel;
=======
            int maxPrecipitation = 100;

            float rawValue = planet.BiomeGenerator.BiomeNoiseGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y);

            int height = blockIndex.Z - planet.BiomeGenerator.SeaLevel;
>>>>>>> feature/performance
            float precipitationDecreasePerBlock = 1;

            return (int)(((1 - rawValue) * maxPrecipitation) - (Math.Max(height, 0) * precipitationDecreasePerBlock));
        }
    }
}
