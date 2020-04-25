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