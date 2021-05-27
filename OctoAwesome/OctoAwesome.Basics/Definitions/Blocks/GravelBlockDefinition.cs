using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GravelBlockDefinition : BlockDefinition
    {
        public GravelBlockDefinition(GravelMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => Languages.OctoBasics.Gravel;

        public override string Icon => "gravel";


        public override string[] Textures { get; } = new[] {"gravel"};

        public override IMaterialDefinition Material { get; }
    }
}