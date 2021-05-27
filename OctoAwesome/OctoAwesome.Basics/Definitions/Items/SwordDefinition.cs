using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    class SwordDefinition : IItemDefinition
    {
        public SwordDefinition()
        {
            Name = "Sword";
            Icon = "sword_iron";
        }

        public string Name { get; }
        public string Icon { get; }

        public bool CanMineMaterial(IMaterialDefinition material)
        {
            return false;
        }

        public Item Create(IMaterialDefinition material)
        {
            return new Sword(this, material);
        }
    }
}