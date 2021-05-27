using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

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

        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager,
            int x, int y, int z)
        {
<<<<<<< HEAD
            var orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);
=======
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);
>>>>>>> feature/performance

            switch (wall)
            {
                case Wall.Top:
                    {
<<<<<<< HEAD
                        var topBlock = manager.GetBlock(x, y, z + 1);
=======
                        ushort topblock = manager.GetBlock(x, y, z + 1);
>>>>>>> feature/performance

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
<<<<<<< HEAD
                                return topBlock != 0 ? 0 : 2;
=======
                                if (topblock != 0)
                                    return 0;
                                else
                                    return 2;
>>>>>>> feature/performance
                        }
                    }
                case Wall.Bottom:
                    {
<<<<<<< HEAD
                        var topBlock = manager.GetBlock(x, y, z + 1);
=======
                        ushort topblock = manager.GetBlock(x, y, z + 1);
>>>>>>> feature/performance

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
<<<<<<< HEAD
                                return topBlock != 0 ? 0 : 2;
=======
                                if (topblock != 0)
                                    return 0;
                                else
                                    return 2;
>>>>>>> feature/performance
                        }
                    }

                case Wall.Front:
                    {
<<<<<<< HEAD
                        var topBlock = manager.GetBlock(x, y, z + 1);
=======
                        ushort topblock = manager.GetBlock(x, y, z + 1);
>>>>>>> feature/performance

                        switch (orientation)
                        {
                            case OrientationFlags.SideSouth:
                            case OrientationFlags.SideNorth:
<<<<<<< HEAD
                                return topBlock != 0 ? 0 : 2;
=======
                                if (topblock != 0)
                                    return 0;
                                else
                                    return 2;
>>>>>>> feature/performance
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
<<<<<<< HEAD
                        var topBlock = manager.GetBlock(x, y, z + 1);
=======
                        ushort topblock = manager.GetBlock(x, y, z + 1);
>>>>>>> feature/performance

                        switch (orientation)
                        {
                            case OrientationFlags.SideSouth:
                            case OrientationFlags.SideNorth:
<<<<<<< HEAD
                                return topBlock != 0 ? 0 : 2;
=======
                                if (topblock != 0)
                                    return 0;
                                else
                                    return 2;
>>>>>>> feature/performance
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
<<<<<<< HEAD
                        var topBlock = manager.GetBlock(x, y, z + 1);
=======
                        ushort topblock = manager.GetBlock(x, y, z + 1);
>>>>>>> feature/performance

                        switch (orientation)
                        {
                            case OrientationFlags.SideWest:
                            case OrientationFlags.SideEast:
<<<<<<< HEAD
                                return topBlock != 0 ? 0 : 2;
=======
                                if (topblock != 0)
                                    return 0;
                                else
                                    return 2;
>>>>>>> feature/performance
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
<<<<<<< HEAD
                        var topBlock = manager.GetBlock(x, y, z + 1);
=======
                        ushort topblock = manager.GetBlock(x, y, z + 1);
>>>>>>> feature/performance

                        switch (orientation)
                        {
                            case OrientationFlags.SideWest:
                            case OrientationFlags.SideEast:
<<<<<<< HEAD
                                return topBlock != 0 ? 0 : 2;
=======
                                if (topblock != 0)
                                    return 0;
                                else
                                    return 2;
>>>>>>> feature/performance
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
<<<<<<< HEAD
            var orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);
=======

            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);
>>>>>>> feature/performance

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

        public override IMaterialDefinition Material { get; }

<<<<<<< HEAD
        public CactusBlockDefinition(CactusMaterialDefinition material) => Material = material;
=======
     
>>>>>>> feature/performance
    }
}
