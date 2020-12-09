using OctoAwesome.Basics.Definitions.Blocks;
using System;

namespace OctoAwesome.Basics.Definitions.Trees
{
    public class SpruceTreeDefinition : TreeDefinition
    {
        private ushort _wood;
        private ushort _leave;
        private ushort _water;

        public override int Order => 15;

        public override float MaxTemperature => 25;

        public override float MinTemperature => -5;

        public override int GetDensity(IPlanet planet, Index3 index) => 4;

        public override void Init(IDefinitionManager definitionManager)
        {
            _wood = definitionManager.GetDefinitionIndex<WoodBlockDefinition>();
            _leave = definitionManager.GetDefinitionIndex<OrangeLeavesBlockDefinition>();
            _water = definitionManager.GetDefinitionIndex<WaterBlockDefinition>();
        }

        public override void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            var ground = builder.GetBlock(0, 0, -1);
            
            if (ground == _water)
                return;

            var rand = new Random(seed);
            var height = rand.Next(3, 5);
            var radius = rand.Next(3, height);

            builder.FillSphere(0, 0, height, radius, _leave);

            var blockInfos = new BlockInfo[height + 2];

            for (var i = 0; i < height + 2; i++)
                blockInfos[i] = (0, 0, i, _wood);

            builder.SetBlocks(blockInfos);
        } 
    }
}
