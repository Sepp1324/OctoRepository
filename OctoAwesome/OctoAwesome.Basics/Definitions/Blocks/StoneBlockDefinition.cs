﻿using System;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class StoneBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Stone;

        public override string Icon => "stone";

        public override string[] Textures => new[] {
                    "stone",
                };

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z) => new PhysicalProperties()
        {
            Density = 2.5f,
            FractureToughness = 0.1f,
            Granularity = 0.1f,
            Hardness = 0.9f
        };

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties) => throw new NotImplementedException();
    }
}
