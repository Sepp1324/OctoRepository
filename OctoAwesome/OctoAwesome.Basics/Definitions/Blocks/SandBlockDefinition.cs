using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class SandBlockDefinition : BlockDefinition
    {
        public SandBlockDefinition(SandMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => Languages.OctoBasics.Sand;

        public override string Icon => "sand";

        public override string[] Textures { get; } = new[] {"sand"};

        public override IMaterialDefinition Material { get; }
    }
}