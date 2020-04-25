using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OctoAwesome.Basics
{
    public sealed class GrassBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Grass"; }
        }

        public IEnumerable<Bitmap> Textures
        {
            get
            {
                return new[] { Resources.grass_top, Resources.grass_bottom, Resources.grass_side };
            }
        }

        public IBlock GetInstance(OrientationFlags orientation)
        {
            return new GrassBlock();
        }

        public Type GetBlockType()
        {
            return typeof(GrassBlock);
        }

        public int GetTextureIndexTop(IBlock block)
        {
            return 0;
        }

        public int GetTextureIndexBottom(IBlock block)
        {
            return 1;
        }

        public int GetTextureIndexNorth(IBlock block)
        {
            return 2;
        }

        public int GetTextureIndexSouth(IBlock block)
        {
            return 2;
        }

        public int GetTextureIndexWest(IBlock block)
        {
            return 2;
        }

        public int GetTextureIndexEast(IBlock block)
        {
            return 2;
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