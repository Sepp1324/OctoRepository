using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public sealed class WaterBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Water"; }
        }

        public IEnumerable<Bitmap> Textures
        {
            get
            {
                return new[] { Resources.water_bottom, Resources.water_side };
            }
        }

        public IBlock GetInstance(OrientationFlags orientation)
        {
            return new WaterBlock();
        }

        public Type GetBlockType()
        {
            return typeof(WaterBlock);
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
            return 1;
        }

        public int GetTextureIndexSouth(IBlock block)
        {
            return 1;
        }

        public int GetTextureIndexWest(IBlock block)
        {
            return 1;
        }

        public int GetTextureIndexEast(IBlock block)
        {
            return 1;
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