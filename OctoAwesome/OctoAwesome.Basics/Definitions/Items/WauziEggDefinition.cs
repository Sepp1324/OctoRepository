namespace OctoAwesome.Basics.Definitions.Items
{
    public sealed class WauziEggDefinition : IItemDefinition
    {
        public string Icon => "wauziegg";

        public string Name => "Wauzi Egg";

        public int StackLimit => 1000;

        public float VolumePerUnit => 1;

        decimal IInventoryableDefinition.VolumePerUnit => 1;
    }
}
