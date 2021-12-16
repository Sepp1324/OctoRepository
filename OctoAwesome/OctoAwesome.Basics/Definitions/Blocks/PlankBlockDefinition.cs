using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class PlankBlockDefinition : BlockDefinition
    {
        public PlankBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => OctoBasics.Plank;

        public override string Icon => "planks_red";

        public override bool HasMetaData => true;

        public override string[] Textures { get; } = { "planks_red" };

        public override IMaterialDefinition Material { get; }
    }
}