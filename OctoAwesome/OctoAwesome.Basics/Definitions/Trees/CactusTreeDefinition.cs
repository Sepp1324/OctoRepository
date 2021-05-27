using System;
using OctoAwesome.Basics.Definitions.Blocks;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Trees
{
    public class CactusTreeDefinition : TreeDefinition
    {
        private ushort cactus, water;

        public override float MaxTemperature => 45;

        public override float MinTemperature => 32;

        public override int Order => 20;

        public override int GetDensity(IPlanet planet, Index3 index)
        {
            return 2;
        }

        public override void Init(IDefinitionManager definitionManager)
        {
            cactus = definitionManager.GetDefinitionIndex<CactusBlockDefinition>();
            water = definitionManager.GetDefinitionIndex<WaterBlockDefinition>();
        }

        public override void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            var ground = builder.GetBlock(0, 0, -1);
            if (ground == water) return;

            var rand = new Random(seed);
            var height = rand.Next(2, 4);

            var infos = new BlockInfo[height];
            for (var i = 0; i < height; i++) infos[i] = (0, 0, i, cactus);

            builder.SetBlocks(false, infos);
        }
    }
}