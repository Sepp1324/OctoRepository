using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class BucketDefinition : IItemDefinition
    {
        public string Name { get; }
        
        public string Icon { get; }

        public BucketDefinition()
        {
            Name = "Bucket";
            Icon = "bucket";
        }
        
        public bool CanMineMaterial(IMaterialDefinition material) => material is IFluidMaterialDefinition fluid;

        public Item Create(IMaterialDefinition material) => new Bucket(this, material);
    }
}
