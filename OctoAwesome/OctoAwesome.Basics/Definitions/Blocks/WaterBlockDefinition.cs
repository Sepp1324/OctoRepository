﻿using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class WaterBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Water;

        public override uint SolidWall => 0;

        public override string Icon => "water";

<<<<<<< HEAD
        public override IMaterialDefinition Material { get; }

        public WaterBlockDefinition(WaterMaterialDefinition material) => Material = material;

        public override string[] Textures => new[] { "water" };
=======
        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 1f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }
     

        public override string[] Textures => new[] {
                    "water"
                };



>>>>>>> feature/performance
    }
}
