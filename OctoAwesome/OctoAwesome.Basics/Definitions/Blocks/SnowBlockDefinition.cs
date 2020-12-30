using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public class SnowBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Snow;

        public override string Icon => "snow";

        public override string[] Textures
        {
            get
            {
                return new[] {
                    "snow",
                    "dirt",
                    "dirt_snow",
                };
            }
        }

        public override IMaterialDefinition GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new IMaterialDefinition()
            {
                Density = 1.5f,
                FractureToughness = 0.2f,
                Granularity = 0.9f,
                Hardness = 0.05f
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
