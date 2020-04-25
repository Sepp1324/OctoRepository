using OctoAwesome.Basics.Properties;
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