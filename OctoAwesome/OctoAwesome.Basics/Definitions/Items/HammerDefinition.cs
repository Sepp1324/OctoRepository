﻿using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    internal class HammerDefinition : IItemDefinition
    {
        public HammerDefinition()
        {
            Name = "Hammer";
            Icon = "hammer_iron";
        }

        public string Name { get; }

        public string Icon { get; }

        public bool CanMineMaterial(IMaterialDefinition material) => false;

        public Item Create(IMaterialDefinition material) => new Hammer(this, material);
    }
}