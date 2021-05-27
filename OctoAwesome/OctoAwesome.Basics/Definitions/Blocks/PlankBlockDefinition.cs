using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class PlankBlockDefinition : BlockDefinition
    {
        public PlankBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => Languages.OctoBasics.Plank;

        public override string Icon => "planks_red";

        public override bool HasMetaData => true;

        public override string[] Textures { get; } = new[] {"planks_red"};

        public override IMaterialDefinition Material { get; }
    }
}