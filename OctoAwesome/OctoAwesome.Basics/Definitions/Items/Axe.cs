using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class Axe : Item
    {
        public Axe(AxeDefinition definition, IMaterialDefinition materialDefinition) : base(definition, materialDefinition)
        {

        }

        public override int Hit(IMaterialDefinition material, decimal blockVolumeVolumeRemaining, int volumePerHit)
        {
            return base.Hit(material, blockVolumeVolumeRemaining, volumePerHit);
        }
    }
}
