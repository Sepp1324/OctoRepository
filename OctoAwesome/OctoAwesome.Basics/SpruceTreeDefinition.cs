﻿using System;

namespace OctoAwesome.Basics
{
    public class SpruceTreeDefinition : TreeDefinition
    {
        private ushort wood;
        private ushort leave;
        private ushort water;

        public override int Order
        {
            get
            {
                return 15;
            }
        }

        public override int GetDensity(IPlanet planet, Index3 index)
        {
            return 5;
        }

        public override void Init(IDefinitionManager definitionManager)
        {
            wood = definitionManager.GetBlockDefinitionIndex<WoodBlockDefinition>();
            leave = definitionManager.GetBlockDefinitionIndex<RedCottonBlockDefinition>();
            water = definitionManager.GetBlockDefinitionIndex<WaterBlockDefinition>();
        }

        public override void PlantTree(IDefinitionManager definitionManager, IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            ushort ground = builder.GetBlock(0, 0, -1);
            if (ground == water) return;

            Random rand = new Random(seed);
            int height = rand.Next(2, 5);
            int radius = rand.Next(2, height);

            builder.FillSphere(0, 0, height, radius, leave);

            for (int i = 0; i < height + 2; i++)
            {
                builder.SetBlock(0, 0, 0 + i, wood);
            }
        }
    }
}