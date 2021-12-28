using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    internal class HoeDefinition : IItemDefinition
    {
        public HoeDefinition()
        {
            Name = "Hoe";
            Icon = "hoe_iron";
        }

        public string Name { get; }

        public string Icon { get; }

        public bool CanMineMaterial(IMaterialDefinition material) => false;

        public Item Create(IMaterialDefinition material) => new Hoe(this, material);
    }
}