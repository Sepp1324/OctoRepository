using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class Bucket : Item, IFluidInventory
    {
        public int MaxQuantity { get; }
        
        public int Quantity { get; private set; }
        
        public IBlockDefinition Fluid { get; private set; }
        
        public Bucket(BucketDefinition definition, IMaterialDefinition materialDefinition) : base(definition, materialDefinition) => MaxQuantity = 125;

        public void AddFluid(int quantity, IBlockDefinition fluid)
        {
            if (!Definition.CanMineMaterial(fluid.Material)) return;
            
            Quantity += quantity;
            Fluid = fluid;
        }

        public override int Hit(IMaterialDefinition material, decimal blockVolumeVolumeRemaining, int volumePerHit)
        {
            if (!Definition.CanMineMaterial(material)) return 0;

            if (material is IFluidMaterialDefinition fluid)
            {
                if(!(Fluid is null) && fluid != Fluid) return 0;

                if (Quantity + volumePerHit >= MaxQuantity) return MaxQuantity - Quantity;

                return volumePerHit;
            }
            
            return base.Hit(material, blockVolumeVolumeRemaining, volumePerHit);
        }
    }
}
