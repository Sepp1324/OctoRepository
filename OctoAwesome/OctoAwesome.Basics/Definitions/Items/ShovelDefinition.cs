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

        public bool CanMineMaterial(IMaterialDefinition material) => material is ISolidMaterialDefinition solid;

        public Item Create(IMaterialDefinition material) => new Shovel(this, material);
    }
}