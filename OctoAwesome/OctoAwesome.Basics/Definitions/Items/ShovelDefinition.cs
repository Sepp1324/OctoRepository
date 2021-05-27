using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    internal class ShovelDefinition : IItemDefinition
    {
        public ShovelDefinition()
        {
            Name = "Shovel";
            Icon = "shovel_iron";
        }

        public string Name { get; }
        public string Icon { get; }

        public bool CanMineMaterial(IMaterialDefinition material)
        {
            if (material is ISolidMaterialDefinition solid) return true;

            return false;
        }

        public Item Create(IMaterialDefinition material)
        {
            return new Shovel(this, material);
        }
    }
}