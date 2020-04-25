﻿using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OctoAwesome.Basics
{
    public sealed class GroundBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Ground"; }
        }

        public IEnumerable<Bitmap> Textures
        {
            get
            {
                return new[] { Resources.ground_bottom };
            }
        }

        public IBlock GetInstance(OrientationFlags orientation)
        {
            return new GroundBlock();
        }

        public Type GetBlockType()
        {
            return typeof(GroundBlock);
        }

        public int GetTextureIndexTop(IBlock block)
        {
            throw new NotImplementedException();
        }

        public int GetTextureIndexBottom(IBlock block)
        {
            throw new NotImplementedException();
        }

        public int GetTextureIndexNorth(IBlock block)
        {
            throw new NotImplementedException();
        }

        public int GetTextureIndexSouth(IBlock block)
        {
            throw new NotImplementedException();
        }

        public int GetTextureIndexWest(IBlock block)
        {
            throw new NotImplementedException();
        }

        public int GetTextureIndexEast(IBlock block)
        {
            throw new NotImplementedException();
        }
    }
}