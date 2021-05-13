using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class SwordDefinition : IItemDefinition
    {
        public string Name { get; }
        
        public string Icon { get; }

        public SwordDefinition()
        {
            Name = "Sword";
            Icon = "sword_iron";
        }

        public bool CanMineMaterial(IMaterialDefinition material) => false;

        public Item Create(IMaterialDefinition material) => new Sword(this, material);
    }
}
