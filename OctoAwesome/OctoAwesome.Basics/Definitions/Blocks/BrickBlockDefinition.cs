using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class BrickBlockDefinition : BlockDefinition
    {
        public BrickBlockDefinition(BrickMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => OctoBasics.Brick;

        public override string Icon => "brick_red";


        public override string[] Textures { get; } = {"brick_red"};

        public override IMaterialDefinition Material { get; }
    }
}