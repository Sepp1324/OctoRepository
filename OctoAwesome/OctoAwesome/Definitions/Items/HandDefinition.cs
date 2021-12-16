using OctoAwesome.Information;

namespace OctoAwesome.Definitions.Items
{
    public class HandDefinition : IItemDefinition
    {
        private readonly Hand hand;

        public HandDefinition()
        {
            VolumePerUnit = 0;
            StackLimit = 0;
            Name = nameof(Hand);
            Icon = "";
            hand = new Hand(this);
        }

        public int VolumePerUnit { get; }

        public int StackLimit { get; }

        public string Name { get; }

        public string Icon { get; }

        public bool CanMineMaterial(IMaterialDefinition material)
        {
            return true;
        }

        public Item Create(IMaterialDefinition material)
        {
            return hand;
        }

        public void Hit(IItem item, IBlockDefinition blockDefinition, BlockHitInformation blockHit)
        {
        }
    }
}