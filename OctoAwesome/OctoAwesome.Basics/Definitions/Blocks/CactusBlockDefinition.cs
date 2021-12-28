using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Definition for Cactus
    /// </summary>
    public class CactusBlockDefinition : BlockDefinition
    {
        /// <summary>
        /// Definition for Cactus
        /// </summary>
        public CactusBlockDefinition() => Textures = new[] { "cactus_inside", "cactus_side", "cactus_top" };

        /// <summary>
        /// Definition for Cactus
        /// </summary>
        /// <param name="material"><see cref="CactusMaterialDefinition"/></param>
        public CactusBlockDefinition(CactusMaterialDefinition material) : this() => Material = material;

        /// <summary>
        /// Material-Inventory Icon
        /// </summary>
        public override string Icon => "cactus_inside";

        /// <summary>
        /// Material-Name
        /// </summary>
        public override string Name => OctoBasics.Cactus;

        /// <summary>
        /// Material-Textures
        /// </summary>
        public override string[] Textures { get; }

        /// <summary>
        /// Material
        /// </summary>
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Returns Texture-Index (Orientation)
        /// </summary>
        /// <param name="wall">Side of <see cref="Wall"/></param>
        /// <param name="manager">Current <see cref="ILocalChunkCache"/></param>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <param name="z">Z-Coordinate</param>
        /// <returns></returns>
        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            var orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

            switch (wall)
            {
                case Wall.Top:
                {
                    var topBlock = manager.GetBlock(x, y, z + 1);

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
                            return topBlock != 0 ? 0 : 2;
                    }
                }
                case Wall.Bottom:
                {
                    var topBlock = manager.GetBlock(x, y, z + 1);

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
                            return topBlock != 0 ? 0 : 2;
                    }
                }

                case Wall.Front:
                {
                    var topBlock = manager.GetBlock(x, y, z + 1);

                    switch (orientation)
                    {
                        case OrientationFlags.SideSouth:
                        case OrientationFlags.SideNorth:
                            return topBlock != 0 ? 0 : 2;
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
                    var topBlock = manager.GetBlock(x, y, z + 1);

                    switch (orientation)
                    {
                        case OrientationFlags.SideSouth:
                        case OrientationFlags.SideNorth:
                            return topBlock != 0 ? 0 : 2;
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
                    var topBlock = manager.GetBlock(x, y, z + 1);

                    switch (orientation)
                    {
                        case OrientationFlags.SideWest:
                        case OrientationFlags.SideEast:
                            return topBlock != 0 ? 0 : 2;
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
                    var topBlock = manager.GetBlock(x, y, z + 1);

                    switch (orientation)
                    {
                        case OrientationFlags.SideWest:
                        case OrientationFlags.SideEast:
                            return topBlock != 0 ? 0 : 2;
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
            var orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

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