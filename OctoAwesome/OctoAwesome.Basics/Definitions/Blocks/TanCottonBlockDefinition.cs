using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class TanCottonBlockDefinition : BlockDefinition
    {
        public TanCottonBlockDefinition(CottonMaterialDefinition material) => Material = material;

        public override string Name => OctoBasics.TanCotton;

        public override string Icon => "cotton_tan";

        public override string[] Textures { get; } = { "cotton_tan" };

        public override IMaterialDefinition Material { get; }
    }
}