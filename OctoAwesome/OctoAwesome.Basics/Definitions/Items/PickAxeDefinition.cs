using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class PickaxeDefinition : IItemDefinition
    {
        public float VolumePerUnit => 10;

        public string Icon => "pick_iron";

        public string Name => "Pickaxe";

        public int StackLimit => 1;

        decimal IInventoryableDefinition.VolumePerUnit => 1;

        public PhysicalProperties GetProperties(IItem item)
        {
            return new PhysicalProperties()
            {
                Density = 1f,
                FractureToughness = 1f,
                Granularity = 1f,
                Hardness = 1f
            };
        }

        public void Hit(IItem item, PhysicalProperties itemProperties)
        {
            // item.Condition--;
        }
    }
}