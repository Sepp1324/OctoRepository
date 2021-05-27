using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GreenCottonBlockDefinition : BlockDefinition
    {
        public GreenCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => OctoBasics.GreenCotton;

        public override string Icon => "cotton_green";


        public override string[] Textures { get; } = {"cotton_green"};

        public override IMaterialDefinition Material { get; }
    }
}