using OctoAwesome.Basics.Definitions.Blocks;
using System;

namespace OctoAwesome.Basics.Definitions.Trees
{
    public class OakTreeDefinition : TreeDefinition
    {
        private ushort _wood;
        private ushort _leave;
        private ushort _water;

        public override int Order => 10;

        public override float MaxTemperature => 27;

        public override float MinTemperature => -5;

        public override void Init(IDefinitionManager definitionManager)
        {
            _wood = definitionManager.GetDefinitionIndex<WoodBlockDefinition>();
            _leave = definitionManager.GetDefinitionIndex<LeavesBlockDefinition>();
            _water = definitionManager.GetDefinitionIndex<WaterBlockDefinition>();
        }

        public override int GetDensity(IPlanet planet, Index3 index) => 4;

        public override void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            var ground = builder.GetBlock(0, 0, -1);

            if (ground == _water)
                return;

            var rand = new Random(seed);
            var height = rand.Next(6, 10);
            var radius = rand.Next(3, height - 2);

            builder.FillSphere(0, 0, height, radius, _leave);

            for (var i = 0; i < height + 2; i++)
                builder.SetBlock(0, 0, 0 + i, _wood);
        }
    }
}
