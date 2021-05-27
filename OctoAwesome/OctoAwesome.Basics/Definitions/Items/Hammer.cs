using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    class Hammer : Item
    {
        public Hammer(HammerDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {
        }
    }
}