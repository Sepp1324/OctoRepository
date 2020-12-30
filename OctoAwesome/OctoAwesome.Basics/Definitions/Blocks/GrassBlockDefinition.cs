using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GrassBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Grass;

        public override string Icon => "grass_top";

        public override string[] Textures
        {
            get
            {
                return new[] {
                    "grass_top",
                    "dirt",
                    "dirt_grass",
                };
            }
        }

        public override IMaterialDefinition GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new IMaterialDefinition
            {
                Density = 2f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }

        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            switch (wall)
            {
                case Wall.Top:
                    return 0;
                case Wall.Bottom:
                    return 1;
                default:
                    return 2;
            }
        }
    }
}
