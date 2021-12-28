using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    internal class BucketDefinition : IItemDefinition
    {
        public BucketDefinition()
        {
            Name = "Bucket";
            Icon = "bucket";
        }

        public string Name { get; }

        public string Icon { get; }

        public bool CanMineMaterial(IMaterialDefinition material) => material is IFluidMaterialDefinition fluid;

        public Item Create(IMaterialDefinition material) => new Bucket(this, material);
    }
}