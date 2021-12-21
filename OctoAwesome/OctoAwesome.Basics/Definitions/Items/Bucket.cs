using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    internal class Bucket : Item, IFluidInventory
    {
        public Bucket(BucketDefinition definition, IMaterialDefinition materialDefinition) : base(definition, materialDefinition) => MaxQuantity = 125;

        public int Quantity { get; private set; }

        public IBlockDefinition FluidBlock { get; private set; }

        public int MaxQuantity { get; }

        public void AddFluid(int quantity, IBlockDefinition fluidBlock)
        {
            if (!Definition.CanMineMaterial(fluidBlock.Material))
                return;

            Quantity += quantity;
            FluidBlock = fluidBlock;
        }

        public override int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining, int volumePerHit)
        {
            if (!Definition.CanMineMaterial(material))
                return 0;

            if (material is not IFluidMaterialDefinition fluid)
                return base.Hit(material, blockInfo, volumeRemaining, volumePerHit);

            if (FluidBlock is not null && fluid != FluidBlock.Material)
                return 0;

            if (Quantity + volumePerHit >= MaxQuantity)
                return MaxQuantity - Quantity;

            return volumePerHit;
        }
    }
}