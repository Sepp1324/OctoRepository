﻿using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class BrickBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Brick;

        public override string Icon => "brick_red";


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "brick_red",
                };
            }
        }

        public override IMaterialDefinition GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new IMaterialDefinition()
            {
                Density = 2.5f,
                FractureToughness = 0.1f,
                Granularity = 0.1f,
                Hardness = 0.9f
            };
        }
    }
}
