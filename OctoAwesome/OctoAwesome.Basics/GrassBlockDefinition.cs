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