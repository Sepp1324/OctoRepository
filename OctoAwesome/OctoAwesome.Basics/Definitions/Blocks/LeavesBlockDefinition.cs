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
    public sealed class LeavesBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Leaves; }
        }

        public override string Icon
        {
            get { return "leaves"; }
        }


<<<<<<< HEAD
        public override string[] Textures =>
            new[] {
                "leaves"
=======
        public override string[] Textures
        {
            get
            {
                return new[] {
                    "leaves"
                };
            }
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 2f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
>>>>>>> feature/performance
            };

<<<<<<< HEAD
        public override IMaterialDefinition Material { get; }

        public LeavesBlockDefinition(LeaveMaterialDefinition material) => Material = material;
=======
       
>>>>>>> feature/performance
    }
}
