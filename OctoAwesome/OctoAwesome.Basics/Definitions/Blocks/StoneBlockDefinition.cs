using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class StoneBlockDefinition : BlockDefinition
    {
        public StoneBlockDefinition(StoneMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => OctoBasics.Stone;

        public override string Icon => "stone";


        public override string[] Textures { get; } = { "stone" };

        public override IMaterialDefinition Material { get; }
    }
}