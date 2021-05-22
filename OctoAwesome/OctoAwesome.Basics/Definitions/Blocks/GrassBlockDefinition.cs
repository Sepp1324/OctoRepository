using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GrassBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Grass;

        public override string Icon => "grass_top";

        public override string[] Textures =>
            new[] {
                "grass_top",
                "dirt",
                "dirt_grass",
            };

        public override IMaterialDefinition Material { get; }

        public GrassBlockDefinition(DirtMaterialDefinition material) => Material = material;

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
