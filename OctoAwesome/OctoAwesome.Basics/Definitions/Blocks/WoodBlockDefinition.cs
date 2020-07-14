using System;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class WoodBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Wood;

        public override string Icon => "wood_top";

        public override bool HasMetaData { get { return true; } }

        public override string[] Textures => new[] {
                "wood_top",
                "wood_side" };

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z) => new PhysicalProperties()
        {
            Density = 0.87f,
            FractureToughness = 0.3f,
            Granularity = 0.9f,
            Hardness = 0.1f
        };

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties) => throw new NotImplementedException();

        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

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
                                return 0;
                        }
                    }

                case Wall.Back:
                case Wall.Front:
                    {
                        switch (orientation)
                        {
                            case OrientationFlags.SideSouth:
                            case OrientationFlags.SideNorth:
                                return 0;
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
                                return 0;
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
    }
}
