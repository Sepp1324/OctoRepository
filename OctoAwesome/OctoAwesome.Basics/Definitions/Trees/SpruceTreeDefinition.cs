using OctoAwesome.Basics.Definitions.Blocks;
using OctoAwesome.Definitions;
using System;

namespace OctoAwesome.Basics.Definitions.Trees
{
    public class SpruceTreeDefinition : TreeDefinition
    {
<<<<<<< HEAD
        private ushort _wood;
        private ushort _leave;
        private ushort _water;
=======
        private ushort wood;
        private ushort leave;
        private ushort water;
>>>>>>> feature/performance

        public override int Order
        {
            get
            {
                return 15;
            }
        }

        public override float MaxTemperature
        {
            get
            {
                return 25;
            }
        }

        public override float MinTemperature
        {
            get
            {
                return -5;
            }
        }

        public override int GetDensity(IPlanet planet, Index3 index) => 4;

        public override void Init(IDefinitionManager definitionManager)
        {
            _wood = definitionManager.GetDefinitionIndex<WoodBlockDefinition>();
            _leave = definitionManager.GetDefinitionIndex<OrangeLeavesBlockDefinition>();
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
            int height = rand.Next(3, 5);
            int radius = rand.Next(3, height);

            builder.FillSphere(0, 0, height, radius, _leave);

            var infos = new BlockInfo[height + 2];
<<<<<<< HEAD
            
            for (var i = 0; i < height + 2; i++)
                infos[i] = (0, 0, i, _wood);
            
=======
            for (int i = 0; i < height + 2; i++)
            {
                infos[i] = (0, 0, i, wood);
            }
>>>>>>> feature/performance
            builder.SetBlocks(false, infos);
         
        }
    }
}
