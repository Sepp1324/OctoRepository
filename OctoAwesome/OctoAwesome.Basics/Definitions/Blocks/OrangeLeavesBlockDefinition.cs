﻿using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class OrangeLeavesBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.OrangeLeaves; }
        }

        public override string Icon
        {
            get { return "leaves_orange"; }
        }


        public override string[] Textures =>
            new[]
            {
<<<<<<< HEAD
                "leaves_orange"
=======
                return new[] {
                    "leaves_orange"
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

        public OrangeLeavesBlockDefinition(LeaveMaterialDefinition material) => Material = material;
=======
       
>>>>>>> feature/performance
    }
}
