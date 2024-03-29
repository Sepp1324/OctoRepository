﻿using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public class SnowBlockDefinition : BlockDefinition
    {
        public SnowBlockDefinition(SnowMaterialDefinition material) => Material = material;

        public override string Name => OctoBasics.Snow;

        public override string Icon => "snow";

        public override string[] Textures { get; } = { "snow", "dirt", "dirt_snow" };

        public override IMaterialDefinition Material { get; }

        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            return wall switch
            {
                Wall.Top => 0,
                Wall.Bottom => 1,
                _ => 2
            };
        }
    }
}