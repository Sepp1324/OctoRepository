using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GreenCottonBlockDefinition : BlockDefinition
    {
        public GreenCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => Languages.OctoBasics.GreenCotton;

        public override string Icon => "cotton_green";


        public override string[] Textures { get; } = new[] {"cotton_green"};

        public override IMaterialDefinition Material { get; }
    }
}