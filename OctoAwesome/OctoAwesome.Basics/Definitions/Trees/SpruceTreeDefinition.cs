using System;
using OctoAwesome.Basics.Definitions.Blocks;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Trees
{
    public class SpruceTreeDefinition : TreeDefinition
    {
        private ushort leave;
        private ushort water;
        private ushort wood;

        public override int Order => 15;

        public override float MaxTemperature => 25;

        public override float MinTemperature => -5;

        public override int GetDensity(IPlanet planet, Index3 index)
        {
            return 4;
        }

        public override void Init(IDefinitionManager definitionManager)
        {
            wood = definitionManager.GetDefinitionIndex<WoodBlockDefinition>();
            leave = definitionManager.GetDefinitionIndex<OrangeLeavesBlockDefinition>();
            water = definitionManager.GetDefinitionIndex<WaterBlockDefinition>();
        }

        public override void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            var ground = builder.GetBlock(0, 0, -1);
            if (ground == water) return;

            var rand = new Random(seed);
            var height = rand.Next(3, 5);
            var radius = rand.Next(3, height);

            builder.FillSphere(0, 0, height, radius, leave);

            var infos = new BlockInfo[height + 2];
            for (var i = 0; i < height + 2; i++) infos[i] = (0, 0, i, wood);

            builder.SetBlocks(false, infos);
        }
    }
}