using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class BlueCottonBlockDefinition : BlockDefinition
    {
        public BlueCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => OctoBasics.BlueCotton;

        public override string Icon => "cotton_blue";


        public override string[] Textures { get; } = {"cotton_blue"};

        public override IMaterialDefinition Material { get; }
    }
}