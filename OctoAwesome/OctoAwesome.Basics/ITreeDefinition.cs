﻿namespace OctoAwesome.Basics
{
    interface ITreeDefinition
    {
        void Init(IDefinitionManager definitionManager);

        int GetDensity(IPlanet planet, Index3 index);

        void PlantTree(IDefinitionManager definitionManager, IPlanet planet, Index3 index, LocalBuilder builder, int seed);
    }
}