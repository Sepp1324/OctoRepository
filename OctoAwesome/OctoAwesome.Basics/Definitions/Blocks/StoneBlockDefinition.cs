using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class StoneBlockDefinition : BlockDefinition
    {
        public StoneBlockDefinition(StoneMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => Languages.OctoBasics.Stone;

        public override string Icon => "stone";


        public override string[] Textures { get; } = new[] {"stone"};

        public override IMaterialDefinition Material { get; }
    }
}