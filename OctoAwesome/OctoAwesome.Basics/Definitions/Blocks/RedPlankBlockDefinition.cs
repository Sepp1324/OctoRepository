﻿using System;
using System.Drawing;

namespace OctoAwesome.Basics
{
    public sealed class RedPlankBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.RedPlank; }
        }

        public override Bitmap Icon
        {
            get { return (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/Blocks/planks.png"); }
        }

        public override bool HasMetaData { get { return true; } }

        public override Bitmap[] Textures
        {
            get
            {
                return new[] {
                (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/Blocks/planks.png")};
            }
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 0.87f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties)
        {
            throw new NotImplementedException();
        }

    }
}