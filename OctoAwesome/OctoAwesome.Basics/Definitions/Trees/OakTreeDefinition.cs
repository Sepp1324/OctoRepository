﻿using OctoAwesome.Basics.Definitions.Blocks;
using System;

namespace OctoAwesome.Basics.Definitions.Trees
{
    public class OakTreeDefinition : TreeDefinition
    {
        private ushort wood;
        private ushort leave;
        private ushort water;

        public override int Order => 10;

        public override float MaxTemperature => 27;

        public override float MinTemperature => -5;

        public override void Init(IDefinitionManager definitionManager)
        {
            wood = definitionManager.GetDefinitionIndex<WoodBlockDefinition>();
            leave = definitionManager.GetDefinitionIndex<LeavesBlockDefinition>();
            water = definitionManager.GetDefinitionIndex<WaterBlockDefinition>();
        }

        public override int GetDensity(IPlanet planet, Index3 index) => 4;

        public override void PlantTree(IDefinitionManager definitionManager, IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            ushort ground = builder.GetBlock(0, 0, -1);

            if (ground == water) return;

            Random rand = new Random(seed);
            int height = rand.Next(6, 10);
            int radius = rand.Next(3, height - 2);

            builder.FillSphere(0, 0, height, radius, leave);

            for (int i = 0; i < height + 2; i++)
                builder.SetBlock(0, 0, 0 + i, wood);
        }
    }
}
