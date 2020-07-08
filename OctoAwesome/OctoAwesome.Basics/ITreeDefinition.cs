﻿namespace OctoAwesome.Basics
{
    interface ITreeDefinition
    {
        int Order { get; }

        float MaxTemperature { get; }

        float MinTemperature { get; }

        void Init(IDefinitionManager definitionManager);

        int GetDensity(IPlanet planet, Index3 index);

        void PlantTree(IDefinitionManager definitionManager, IPlanet planet, Index3 index, LocalBuilder builder, int seed);
    }
}
