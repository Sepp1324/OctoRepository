﻿using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public class CactusBlockDefinition : BlockDefinition
    {
        public CactusBlockDefinition()
        {
            Textures = new[] {"cactus_inside", "cactus_side", "cactus_top"};
        }

        public CactusBlockDefinition(CactusMaterialDefinition material) : this()
        {
            Material = material;
        }

        public override string Icon => "cactus_inside";

        public override string Name => Languages.OctoBasics.Cactus;

        public override string[] Textures { get; }

        public override IMaterialDefinition Material { get; }

        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager,
            int x, int y, int z)
        {
            var orientation = (OrientationFlags) manager.GetBlockMeta(x, y, z);

            switch (wall)
            {
                case Wall.Top:
                {
                    var topblock = manager.GetBlock(x, y, z + 1);

                    switch (orientation)
                    {
                        case OrientationFlags.SideWest:
                        case OrientationFlags.SideEast:
                        case OrientationFlags.SideSouth:
                        case OrientationFlags.SideNorth:
                            return 1;
                        case OrientationFlags.SideBottom:
                        case OrientationFlags.SideTop:
                        default:
                            if (topblock != 0)
                                return 0;
                            else
                                return 2;
                    }
                }
                case Wall.Bottom:
                {
                    var topblock = manager.GetBlock(x, y, z + 1);

                    switch (orientation)
                    {
                        case OrientationFlags.SideWest:
                        case OrientationFlags.SideEast:
                        case OrientationFlags.SideSouth:
                        case OrientationFlags.SideNorth:
                            return 1;
                        case OrientationFlags.SideBottom:
                        case OrientationFlags.SideTop:
                        default:
                            if (topblock != 0)
                                return 0;
                            else
                                return 2;
                    }
                }

                case Wall.Front:
                {
                    var topblock = manager.GetBlock(x, y, z + 1);

                    switch (orientation)
                    {
                        case OrientationFlags.SideSouth:
                        case OrientationFlags.SideNorth:
                            if (topblock != 0)
                                return 0;
                            else
                                return 2;
                        case OrientationFlags.SideWest:
                        case OrientationFlags.SideEast:
                        case OrientationFlags.SideBottom:
                        case OrientationFlags.SideTop:
                        default:
                            return 1;
                    }
                }
                case Wall.Back:
                {
                    var topblock = manager.GetBlock(x, y, z + 1);

                    switch (orientation)
                    {
                        case OrientationFlags.SideSouth:
                        case OrientationFlags.SideNorth:
                            if (topblock != 0)
                                return 0;
                            else
                                return 2;
                        case OrientationFlags.SideWest:
                        case OrientationFlags.SideEast:
                        case OrientationFlags.SideBottom:
                        case OrientationFlags.SideTop:
                        default:
                            return 1;
                    }
                }

                case Wall.Left:
                {
                    var topblock = manager.GetBlock(x, y, z + 1);

                    switch (orientation)
                    {
                        case OrientationFlags.SideWest:
                        case OrientationFlags.SideEast:
                            if (topblock != 0)
                                return 0;
                            else
                                return 2;
                        case OrientationFlags.SideSouth:
                        case OrientationFlags.SideNorth:
                        case OrientationFlags.SideBottom:
                        case OrientationFlags.SideTop:
                        default:
                            return 1;
                    }
                }

                case Wall.Right:
                {
                    var topblock = manager.GetBlock(x, y, z + 1);

                    switch (orientation)
                    {
                        case OrientationFlags.SideWest:
                        case OrientationFlags.SideEast:
                            if (topblock != 0)
                                return 0;
                            else
                                return 2;
                        case OrientationFlags.SideSouth:
                        case OrientationFlags.SideNorth:
                        case OrientationFlags.SideBottom:
                        case OrientationFlags.SideTop:
                        default:
                            return 1;
                    }
                }
            }

            // Should never happen
            // Assert here
            return -1;
        }

        public override int GetTextureRotation(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            var orientation = (OrientationFlags) manager.GetBlockMeta(x, y, z);

            switch (wall)
            {
                case Wall.Top:
                case Wall.Bottom:
                case Wall.Back:
                case Wall.Front:
                    switch (orientation)
                    {
                        case OrientationFlags.SideWest:
                        case OrientationFlags.SideEast:
                            return 1;
                        case OrientationFlags.SideSouth:
                        case OrientationFlags.SideNorth:
                        case OrientationFlags.SideBottom:
                        case OrientationFlags.SideTop:
                        default:
                            return 0;
                    }
                case Wall.Left:
                case Wall.Right:
                    switch (orientation)
                    {
                        case OrientationFlags.SideSouth:
                        case OrientationFlags.SideNorth:
                            return 1;
                        case OrientationFlags.SideWest:
                        case OrientationFlags.SideEast:
                        case OrientationFlags.SideBottom:
                        case OrientationFlags.SideTop:
                        default:
                            return 0;
                    }
                default:
                    return base.GetTextureRotation(wall, manager, x, y, z); //should never ever happen
            }
        }
    }
}