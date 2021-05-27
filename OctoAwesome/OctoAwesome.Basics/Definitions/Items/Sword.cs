using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    class Sword : Item
    {
        public Sword(SwordDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {
        }
    }
}