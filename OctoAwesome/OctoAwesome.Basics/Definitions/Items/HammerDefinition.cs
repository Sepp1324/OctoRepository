﻿using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class HammerDefinition : IItemDefinition
    {
        public string Name { get; }
        
        public string Icon { get; }

        public HammerDefinition()
        {
            Name = "Hammer";
            Icon = "hammer_iron";
        }

        public bool CanMineMaterial(IMaterialDefinition material) => false;

        public Item Create(IMaterialDefinition material) => new Hammer(this, material);
    }
}