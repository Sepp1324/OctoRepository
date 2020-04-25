﻿
using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OctoAwesome.Basics
{
    public sealed class SandBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Sand"; }
        }

        public IEnumerable<Bitmap> Textures
        {
            get
            {
                return new[] { Resources.sand_bottom };
            }
        }

        public IBlock GetInstance(OrientationFlags orientation)
        {
            throw new NotImplementedException();
        }

        public Type GetBlockType()
        {
            return typeof(SandBlock);
        }

        public int GetTextureIndexTop(IBlock block)
        {
            return 0;
        }

        public int GetTextureIndexBottom(IBlock block)
        {
            return 0;
        }

        public int GetTextureIndexNorth(IBlock block)
        {
            return 0;
        }

        public int GetTextureIndexSouth(IBlock block)
        {
            return 0;
        }

        public int GetTextureIndexWest(IBlock block)
        {
            return 0;
        }

        public int GetTextureIndexEast(IBlock block)
        {
            return 0;
        }

        public int GetTextureRotationTop(IBlock block)
        {
            return 0;
        }

        public int GetTextureRotationBottom(IBlock block)
        {
            return 0;
        }

        public int GetTextureRotationNorth(IBlock block)
        {
            return 0;
        }

        public int GetTextureRotationSouth(IBlock block)
        {
            return 0;
        }

        public int GetTextureRotationWest(IBlock block)
        {
            return 0;
        }

        public int GetTextureRotationEast(IBlock block)
        {
            return 0;
        }
    }
}