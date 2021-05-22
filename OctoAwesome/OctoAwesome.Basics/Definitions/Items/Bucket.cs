using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class Bucket : Item, IFluidInventory
    {
        public int Quantity { get; private set; }
        
        public IFluidMaterialDefinition Fluid { get; private set; }
        
        public Bucket(BucketDefinition definition, IMaterialDefinition materialDefinition) : base(definition, materialDefinition)
        {

        }

        public void AddFluid(int quantity, IFluidMaterialDefinition fluid)
        {
            Quantity += quantity;
            Fluid = fluid;
        }

        public override int Hit(IMaterialDefinition material, decimal blockVolumeVolumeRemaining, int volumePerHit)
        {
            if (!Definition.CanMineMaterial(material))
                return 0;

            if (material is IFluidMaterialDefinition fluid)
            {
                if(!(fluid is null) && fluid != Fluid)
                    return 0;

                return volumePerHit;
            }
            
            return base.Hit(material, blockVolumeVolumeRemaining, volumePerHit);
        }
    }
}
