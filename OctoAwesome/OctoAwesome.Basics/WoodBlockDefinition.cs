using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OctoAwesome.Basics
{
    public sealed class WoodBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Wood"; }
        }

        public IEnumerable<Bitmap> Textures
        {
            get { return new[] { Resources.wood_bottom, Resources.wood_side }; }
        }

        public Type GetBlockType()
        {
            return typeof(WoodBlock);
        }

        public IBlock GetInstance(OrientationFlags orientation)
        {
            return new WoodBlock() { Orientation = orientation };
        }

        public int GetTextureIndexBottom(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 1;

                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 0;
            }
        }

        public int GetTextureIndexEast(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 0;

                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 1;
            }
        }

        public int GetTextureIndexNorth(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 0;

                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 1;
            }
        }

        public int GetTextureIndexSouth(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 0;

                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 1;
            }
        }

        public int GetTextureIndexTop(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 1;

                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 0;
            }
        }

        public int GetTextureIndexWest(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 0;

                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 1;
            }
        }

        public int GetTextureRotationTop(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 1;
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 0;
            }
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
