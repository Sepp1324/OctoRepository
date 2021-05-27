using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    class HammerDefinition : IItemDefinition
    {
        public HammerDefinition()
        {
            Name = "Hammer";
            Icon = "hammer_iron";
        }

        public string Name { get; }
        public string Icon { get; }

        public bool CanMineMaterial(IMaterialDefinition material)
        {
            return false;
        }

        public Item Create(IMaterialDefinition material)
        {
            return new Hammer(this, material);
        }
    }
}