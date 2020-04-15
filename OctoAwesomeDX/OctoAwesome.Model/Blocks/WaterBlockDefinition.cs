using OctoAwesome.Model.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model.Blocks
{
    public sealed class WaterBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Water"; }
        }

        public Bitmap TopTexture
        {
            get { return Resources.water_center; }
        }

        public Bitmap BottomTexture
        {
            get { return Resources.water_center; }
        }

        public Bitmap SideTexture
        {
            get { return Resources.water_center; }
        }

        IBlock IBlockDefinition.GetInstance()
        {
            return new WaterBlock();
        }
    }
}
