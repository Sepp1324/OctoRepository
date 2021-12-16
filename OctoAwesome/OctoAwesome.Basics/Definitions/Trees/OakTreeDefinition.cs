using System;
using OctoAwesome.Basics.Definitions.Blocks;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Trees
{
    public class OakTreeDefinition : TreeDefinition
    {
        private ushort leave;
        private ushort water;
        private ushort wood;

        public override int Order => 10;

        public override float MaxTemperature => 27;

        public override float MinTemperature => -5;

        public override void Init(IDefinitionManager definitionManager)
        {
            wood = definitionManager.GetDefinitionIndex<WoodBlockDefinition>();
            leave = definitionManager.GetDefinitionIndex<LeavesBlockDefinition>();
            water = definitionManager.GetDefinitionIndex<WaterBlockDefinition>();
        }

        public override int GetDensity(IPlanet planet, Index3 index)
        {
            return 4;
        }

        public override void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            var ground = builder.GetBlock(0, 0, -1);
            if (ground == water) return;

            var rand = new Random(seed);
            var height = rand.Next(6, 10);
            var radius = rand.Next(3, height - 2);

            builder.FillSphere(0, 0, height, radius, leave);

            var infos = new BlockInfo[height + 2];
            for (var i = 0; i < height + 2; i++) infos[i] = (0, 0, i, wood);
            builder.SetBlocks(false, infos);
        }
    }
}