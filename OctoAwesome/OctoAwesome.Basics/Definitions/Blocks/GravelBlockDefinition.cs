using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GravelBlockDefinition : BlockDefinition
    {
        public GravelBlockDefinition(GravelMaterialDefinition material) => Material = material;

        public override string Name => OctoBasics.Gravel;

        public override string Icon => "gravel";

        public override string[] Textures { get; } = { "gravel" };

        public override IMaterialDefinition Material { get; }
    }
}