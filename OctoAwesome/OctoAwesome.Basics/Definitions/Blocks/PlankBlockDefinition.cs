using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class PlankBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Plank;

        public override string Icon => "planks_red";

        public override bool HasMetaData { get { return true; } }

        public override string[] Textures
        {
            get
            {
                return new[] {
                "planks_red"};
            }
        }

        public override IMaterialDefinition Material { get; }

        public PlankBlockDefinition(WoodMaterialDefinition material) => Material = material;
    }
}
