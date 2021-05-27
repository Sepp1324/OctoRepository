<<<<<<< HEAD
﻿using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;
=======
﻿using OctoAwesome.Information;
using System;
using System.Drawing;
>>>>>>> feature/performance

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class StoneBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Stone; }
        }

        public override string Icon
        {
            get { return "stone"; }
        }

<<<<<<< HEAD
        public override string[] Textures =>
            new[] {
                "stone",
=======

        public override string[] Textures
        {
            get
            {
                return new[] {
                    "stone",
                };
            }
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 2.5f,
                FractureToughness = 0.1f,
                Granularity = 0.1f,
                Hardness = 0.9f
>>>>>>> feature/performance
            };

<<<<<<< HEAD
        public override IMaterialDefinition Material { get; }

        public StoneBlockDefinition(StoneMaterialDefinition material) => Material = material;
    }
=======
          }
>>>>>>> feature/performance
}
