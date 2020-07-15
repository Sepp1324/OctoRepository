using System;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public class SnowBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Snow;

        public override string Icon => "snow";

        public override string[] Textures => new[] {
                    "snow",
                    "dirt",
                    "dirt_snow",
                };

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z) => new PhysicalProperties()
        {
            Density = 1.5f,
            FractureToughness = 0.2f,
            Granularity = 0.9f,
            Hardness = 0.05f
        };

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties) => throw new NotImplementedException();

        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            if (wall == Wall.Top)
                return 0;
            else if (wall == Wall.Bottom)
                return 1;
            else
                return 2;
        }
    }
}
