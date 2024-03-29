﻿using System;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    internal class Shovel : Item
    {
        public Shovel(ShovelDefinition definition, IMaterialDefinition materialDefinition) : base(definition, materialDefinition)
        {
        }

        public override int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining,
            int volumePerHit)
        {
            if (!Definition.CanMineMaterial(material))
                return 0;

            if (material is not ISolidMaterialDefinition { Granularity: > 1 } solid) 
                return 0;

            //if (solid * 1.2f < material.Hardness)
            //    return 0;

            return (int)(Math.Sin(solid.Granularity / 40) * 2 * volumePerHit);
        }
    }
}