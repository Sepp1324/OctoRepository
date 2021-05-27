using OctoAwesome.Information;
<<<<<<< HEAD
=======
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
>>>>>>> feature/performance

namespace OctoAwesome.Definitions.Items
{
    public class HandDefinition : IItemDefinition
    {
<<<<<<< HEAD
        public int VolumePerUnit { get; }

        public int StackLimit { get; }

        public string Name { get; }

        public string Icon { get; }

        private readonly Hand _hand;

        public HandDefinition()
        {
            VolumePerUnit = 0;
            StackLimit = 0;
            Name = nameof(Hand);
            Icon = "";
            _hand = new Hand(this);
        }

        public void Hit(IItem item, IBlockDefinition blockDefinition, BlockHitInformation blockHit) { }

        public bool CanMineMaterial(IMaterialDefinition material) => true;

        public Item Create(IMaterialDefinition material) => _hand;
=======
        public int VolumePerUnit => 0;

        public int StackLimit => 0;

        public string Name => nameof(Hand);

        public string Icon => "";

        public void Hit(IItem item, IBlockDefinition blockDefinition, BlockHitInformation blockHit) { }
>>>>>>> feature/performance
    }
}
