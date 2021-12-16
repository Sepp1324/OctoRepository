using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class RedPlankBlockDefinition : BlockDefinition
    {
        public RedPlankBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => OctoBasics.RedPlank;

        public override string Icon => "planks";

        public override bool HasMetaData => true;

        public override string[] Textures { get; } = { "planks" };

        public override IMaterialDefinition Material { get; }
    }
}