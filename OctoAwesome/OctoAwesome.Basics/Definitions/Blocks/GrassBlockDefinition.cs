using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GrassBlockDefinition : BlockDefinition
    {
        public GrassBlockDefinition(DirtMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => OctoBasics.Grass;

        public override string Icon => "grass_top";

        public override string[] Textures { get; } =
        {
            "grass_top",
            "dirt",
            "dirt_grass"
        };

        public override IMaterialDefinition Material { get; }

        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            if (wall == Wall.Top)
                return 0;
            if (wall == Wall.Bottom)
                return 1;
            return 2;
        }
    }
}