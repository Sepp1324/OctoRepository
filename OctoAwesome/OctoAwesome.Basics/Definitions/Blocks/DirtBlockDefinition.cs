using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class DirtBlockDefinition : BlockDefinition
    {
        public DirtBlockDefinition(DirtMaterialDefinition material) => Material = material;

        public override string Name => OctoBasics.Ground;

        public override string Icon => "dirt";


        public override string[] Textures { get; } = { "dirt" };

        public override IMaterialDefinition Material { get; }
    }
}