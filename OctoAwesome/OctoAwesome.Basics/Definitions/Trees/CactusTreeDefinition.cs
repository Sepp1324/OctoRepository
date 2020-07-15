using OctoAwesome.Basics.Definitions.Blocks;
using System;

namespace OctoAwesome.Basics.Definitions.Trees
{
    public class CactusTreeDefinition : TreeDefinition
    {
        private ushort cactus, water;

        public override float MaxTemperature => 45;

        public override float MinTemperature => 32;

        public override int Order => 20;

        public override int GetDensity(IPlanet planet, Index3 index) => 2;

        public override void Init(IDefinitionManager definitionManager)
        {
            cactus = definitionManager.GetDefinitionIndex<CactusBlockDefinition>();
            water = definitionManager.GetDefinitionIndex<WaterBlockDefinition>();
        }

        public override void PlantTree(IDefinitionManager definitionManager, IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            ushort ground = builder.GetBlock(0, 0, -1);

            if (ground == water) return;

            Random rand = new Random(seed);
            int height = rand.Next(2, 4);

            for (int i = 0; i < height; i++)
                builder.SetBlock(0, 0, i, cactus);
        }
    }
}
