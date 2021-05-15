using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class RedPlankBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.RedPlank;

        public override string Icon => "planks";

        public override bool HasMetaData { get { return true; } }

        public override string[] Textures
        {
            get
            {
                return new[] {
                "planks"};
            }
        }

        public override IMaterialDefinition Material { get; }

        public RedPlankBlockDefinition(WoodMaterialDefinition material) => Material = material;
    }
}
