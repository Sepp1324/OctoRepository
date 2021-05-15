using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class HoeDefinition : IItemDefinition
    {
        public string Name { get; }
        
        public string Icon { get; }

        public HoeDefinition()
        {
            Name = "Hoe";
            Icon = "hoe_iron";
        }

        public bool CanMineMaterial(IMaterialDefinition material) => false;

        public Item Create(IMaterialDefinition material) => new Hoe(this, material);
    }
}
