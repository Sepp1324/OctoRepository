using System;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public class CactusBlockDefinition : BlockDefinition
    {
        public override string Icon => "cactus_inside";

        public override string Name => Languages.OctoBasics.Cactus;

        public override string[] Textures => new[] {
                    "cactus_inside",
                    "cactus_side",
                    "cactus_top"
                };

        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);
            ushort topblock = manager.GetBlock(x, y, z + 1);

            switch (wall)
            {
                case Wall.Top:
                case Wall.Bottom:
                    {
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
                case Wall.Back:
                    {
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
                case Wall.Right:
                    {
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
            return -1;
        }

        public override int GetTopTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

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
        }

        public override int GetBottomTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

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
        }

        public override int GetEastTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

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
        }

        public override int GetWestTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

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
        }

        public override int GetNorthTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

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
        }

        public override int GetSouthTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

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
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 2f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties)
        {
            throw new NotImplementedException();
        }
    }
}
