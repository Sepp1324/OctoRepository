using OctoAwesome.Basics.Definitions.Blocks;
using OctoAwesome.Definitions;
using System;

namespace OctoAwesome.Basics.Definitions.Trees
{
    public class CactusTreeDefinition : TreeDefinition
    {
        private ushort _cactus, _water;

        public override float MaxTemperature
        {
            get { return 45; }
        }

        public override float MinTemperature
        {
            get { return 32; }
        }

        public override int Order
        {
            get { return 20; }
        }

        public override int GetDensity(IPlanet planet, Index3 index) => 2;

        public override void Init(IDefinitionManager definitionManager)
        {
            _cactus = definitionManager.GetDefinitionIndex<CactusBlockDefinition>();
            _water = definitionManager.GetDefinitionIndex<WaterBlockDefinition>();
        }

        public override void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
<<<<<<< HEAD
            var ground = builder.GetBlock(0, 0, -1);
            
            if (ground == _water) return;
=======
            ushort ground = builder.GetBlock(0, 0, -1);
            if (ground == water) return;
>>>>>>> feature/performance

            Random rand = new Random(seed);
            int height = rand.Next(2, 4);

            var infos = new BlockInfo[height ];
<<<<<<< HEAD
            
            for (var i = 0; i < height; i++)
                infos[i] = (0, 0,  i, _cactus);
            
=======
            for (int i = 0; i < height; i++)
            {
                infos[i] = (0, 0,  i, cactus);
            }
>>>>>>> feature/performance
            builder.SetBlocks(false, infos);
        }
    }
}
