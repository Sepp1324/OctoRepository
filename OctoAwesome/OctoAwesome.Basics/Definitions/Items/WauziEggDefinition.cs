using OctoAwesome.Definitions;
using OctoAwesome.Information;

namespace OctoAwesome.Basics.Definitions.Items
{
    public sealed class WauziEggDefinition : IItemDefinition
    {
        public string Icon => "wauziegg";

        public string Name => "Wauzi Egg";

        public int StackLimit => 1000;

        public float VolumePerUnit => 1;

        int IInventoryableDefinition.VolumePerUnit => 1;

        public void Hit(IItem item, IBlockDefinition blockDefinition, BlockHitInformation blockHit) => throw new System.NotImplementedException();
    }
}
