using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class RedCottonBlockDefinition : BlockDefinition
    {
        public RedCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => OctoBasics.RedCotton;

        public override string Icon => "cotton_red";


        public override string[] Textures { get; } = { "cotton_red" };

        public override IMaterialDefinition Material { get; }
    }
}