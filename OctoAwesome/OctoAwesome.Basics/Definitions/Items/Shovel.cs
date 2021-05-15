using System;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class Shovel : Item
    {
        public Shovel(ShovelDefinition definition, IMaterialDefinition materialDefinition) : base(definition, materialDefinition)
        {

        }

        public override int Hit(IMaterialDefinition material, decimal blockVolumeVolumeRemaining, int volumePerHit)
        {
            if (!Definition.CanMineMaterial(material))
                return 0;

            if (material is ISolidMaterialDefinition solid)
            {
                if (solid.Granularity <= 1)
                    return 0;

                return (int) (Math.Sin(solid.Granularity / 40) * 2 * volumePerHit);
            }

            return 0;
        }
    }
}
