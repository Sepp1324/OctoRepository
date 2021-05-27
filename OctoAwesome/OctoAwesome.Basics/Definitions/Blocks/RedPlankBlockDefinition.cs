using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class RedPlankBlockDefinition : BlockDefinition
    {
        public RedPlankBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => Languages.OctoBasics.RedPlank;

        public override string Icon => "planks";

        public override bool HasMetaData => true;

        public override string[] Textures { get; } = new[] {"planks"};

        public override IMaterialDefinition Material { get; }
    }
}