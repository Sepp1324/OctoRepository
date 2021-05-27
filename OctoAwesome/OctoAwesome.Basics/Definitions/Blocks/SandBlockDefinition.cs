using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class SandBlockDefinition : BlockDefinition
    {
        public SandBlockDefinition(SandMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => OctoBasics.Sand;

        public override string Icon => "sand";

        public override string[] Textures { get; } = {"sand"};

        public override IMaterialDefinition Material { get; }
    }
}