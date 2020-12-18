namespace OctoAwesome.Basics.Definitions.Items
{
    public sealed class WauziEggDefinition : IItemDefinition
    {
        public float VolumePerUnit => 1;

        public string Icon => "wauziegg";

        public string Name => "Wauzi Egg";

        public int StackLimit => 1000;

        decimal IInventoryableDefinition.VolumePerUnit => 1;
    }
}