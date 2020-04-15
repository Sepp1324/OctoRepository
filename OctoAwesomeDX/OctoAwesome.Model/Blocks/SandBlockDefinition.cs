using OctoAwesome.Model.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model.Blocks
{
    public sealed class SandBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Sand"; }
        }

        public Bitmap TopTexture
        {
            get { return Resources.sand_center; }
        }

        public Bitmap BottomTexture
        {
            get { return Resources.sand_center; }
        }

        public Bitmap SideTexture
        {
            get { return Resources.sand_center; }
        }

        IBlock IBlockDefinition.GetInstance()
        {
            return new SandBlock();
        }
    }
}
