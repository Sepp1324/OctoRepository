using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public class SnowBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Snow;

        public override string Icon => "snow";

        public override string[] Textures =>
            new[] {
                "snow",
                "dirt",
                "dirt_snow",
            };

        public override IMaterialDefinition Material { get; }

        public SnowBlockDefinition(SnowMaterialDefinition material) => Material = material;

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
