﻿using System;
using OctoAwesome.Basics.Definitions.Blocks;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Trees
{
    public class CactusTreeDefinition : TreeDefinition
    {
        private ushort _cactus, _water;

        public override float MaxTemperature => 45;

        public override float MinTemperature => 32;

        public override int Order => 20;

        public override int GetDensity(IPlanet planet, Index3 index) => 2;

        public override void Init(IDefinitionManager definitionManager)
        {
            _cactus = definitionManager.GetDefinitionIndex<CactusBlockDefinition>();
            _water = definitionManager.GetDefinitionIndex<WaterBlockDefinition>();
        }

        public override void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            var ground = builder.GetBlock(0, 0, -1);
            if (ground == _water) return;

            var rand = new Random(seed);
            var height = rand.Next(2, 4);

            var infos = new BlockInfo[height];
            for (var i = 0; i < height; i++) infos[i] = (0, 0, i, _cactus);
            builder.SetBlocks(false, infos);
        }
    }
}