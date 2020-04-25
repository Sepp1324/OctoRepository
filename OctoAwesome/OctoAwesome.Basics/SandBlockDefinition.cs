
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