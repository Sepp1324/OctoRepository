using OctoAwesome.Model.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model.Blocks
{
    public sealed class StoneBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Stone"; }
        }

        public Bitmap TopTexture
        {
            get { return Resources.stone_center; }
        }

        public Bitmap BottomTexture
        {
            get { return Resources.stone_center; }
        }

        public Bitmap SideTexture
        {
            get { return Resources.stone_center; }
        }

        IBlock IBlockDefinition.GetInstance()
        {
            return new StoneBlock();
        }
    }
}
