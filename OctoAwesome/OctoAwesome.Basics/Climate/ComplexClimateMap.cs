﻿using System;
using OctoAwesome.Noise;

namespace OctoAwesome.Basics.Climate
{
    public class ComplexClimateMap : IClimateMap
    {
        readonly ComplexPlanet planet;
        private readonly INoise tempFluctuationGenerator;

        public ComplexClimateMap(ComplexPlanet planet)
        {
            this.planet = planet;
            tempFluctuationGenerator = new SimplexNoiseGenerator(planet.Seed - 1, 1f / 64, 1f / 64) {Octaves = 3};
        }

        public IPlanet Planet => planet;

        public float GetTemperature(Index3 blockIndex)
        {
            var equator = (Planet.Size.Y * Chunk.CHUNKSIZE_Y) / 2;
            var equatorTemperature = 40f;
            var poleTemperature = -20f;
            var tempFluctuation = tempFluctuationGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y) * 5f;
            var temperatureDifference = poleTemperature - equatorTemperature;
            var temperatureDecreasePerBlock = 0.1f;
            float distance = Math.Abs(blockIndex.Y - equator);
            var temperature = tempFluctuation + equatorTemperature + temperatureDifference * (float) Math.Sin((Math.PI / 2) * distance / equator); //equatorTemperature + distance * temperatureDifference / equator;
            var height = (float) (blockIndex.Z - planet.BiomeGenerator.SeaLevel) / (Planet.Size.Z * Chunk.CHUNKSIZE_Z - planet.BiomeGenerator.SeaLevel);
            height = Math.Max(height, 0);
            height = height * height;
            return temperature - height * temperatureDecreasePerBlock;
        }

        public int GetPrecipitation(Index3 blockIndex)
        {
            var maxPrecipitation = 100;

            var rawValue = planet.BiomeGenerator.BiomeNoiseGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y);

            var height = blockIndex.Z - planet.BiomeGenerator.SeaLevel;
            float precipitationDecreasePerBlock = 1;

            return (int) (((1 - rawValue) * maxPrecipitation) - (Math.Max(height, 0) * precipitationDecreasePerBlock));
        }
    }
}