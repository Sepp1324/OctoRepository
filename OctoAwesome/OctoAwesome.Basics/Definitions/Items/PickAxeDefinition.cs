using OctoAwesome.Definitions;
using OctoAwesome.Information;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class PickaxeDefinition : IItemDefinition
    {
        public string Icon => "pick_iron";

        public string Name => "Pickaxe";

        public int StackLimit => 1;

        public float VolumePerUnit => 10;

        int IInventoryableDefinition.VolumePerUnit => 1;

        public IMaterialDefinition GetProperties(IItem item)
        {
            return new IMaterialDefinition()
            {
                Density = 1f,
                FractureToughness = 1f,
                Granularity = 1f,
                Hardness = 1f
            };
        }

        public void Hit(IItem item, IBlockDefinition blockDefinition, BlockHitInformation blockHit)
        {
            // item.Condition--;
        }
    }
}
