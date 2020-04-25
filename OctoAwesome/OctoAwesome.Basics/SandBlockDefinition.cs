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
                return new[] { Resources.sand_top };
            }
        }

        //public Bitmap TopTexture
        //{
        //    get { return Resources.sand_top; }
        //}

        //public Bitmap BottomTexture
        //{
        //    get { return Resources.sand_bottom; }
        //}

        //public Bitmap SideTexture
        //{
        //    get { return Resources.sand_side; }
        //}

        public IBlock GetInstance()
        {
            throw new NotImplementedException();
        }

        public Type GetBlockType()
        {
            return typeof(SandBlock);
        }
    }
}