using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OctoAwesome.Basics
{
    public sealed class StoneBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Stone"; }
        }

        public IEnumerable<Bitmap> Textures
        {
            get
            {
                return new[] { Resources.stone_bottom, Resources.stone_side };
            }
        }

        public IBlock GetInstance(OrientationFlags orientation)
        {
            return new StoneBlock();
        }
        public Type GetBlockType()
        {
            return typeof(StoneBlock);
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