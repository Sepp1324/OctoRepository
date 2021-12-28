using OctoAwesome.Information;

namespace OctoAwesome.Definitions.Items
{
    /// <summary>
    /// </summary>
    public class HandDefinition : IItemDefinition
    {
        private readonly Hand _hand;

        /// <summary>
        /// </summary>
        public HandDefinition()
        {
            VolumePerUnit = 0;
            StackLimit = 0;
            Name = nameof(Hand);
            Icon = "";
            _hand = new(this);
        }

        private int VolumePerUnit { get; }

        private int StackLimit { get; }

        public string Name { get; }

        public string Icon { get; }

        public bool CanMineMaterial(IMaterialDefinition material) => true;

        public Item Create(IMaterialDefinition material) => _hand;

        public void Hit(IItem item, IBlockDefinition blockDefinition, BlockHitInformation blockHit)
        {
        }
    }
}