using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    class BucketDefinition : IItemDefinition
    {
        public BucketDefinition()
        {
            Name = "Bucket";
            Icon = "bucket";
        }

        public string Name { get; }
        public string Icon { get; }

        public bool CanMineMaterial(IMaterialDefinition material)
        {
            if (material is IFluidMaterialDefinition fluid) return true;

            return false;
        }

        public Item Create(IMaterialDefinition material)
        {
            return new Bucket(this, material);
        }
    }
}