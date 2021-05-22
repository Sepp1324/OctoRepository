using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class Bucket : Item, IFluidInventory
    {
        public int Quantity { get; set; }
        
        public IFluidMaterialDefinition Fluid { get; set; }
        
        public Bucket(BucketDefinition definition, IMaterialDefinition materialDefinition) : base(definition, materialDefinition)
        {

        }

        public void AddFluid(int quantity, IFluidMaterialDefinition fluid)
        {
            Quantity += quantity;
            Fluid = fluid;
        }
    }
}
