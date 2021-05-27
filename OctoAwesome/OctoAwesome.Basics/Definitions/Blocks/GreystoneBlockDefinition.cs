using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GreystoneBlockDefinition : BlockDefinition
    {
        public GreystoneBlockDefinition(StoneMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => Languages.OctoBasics.Greystone;

        public override string Icon => "greystone";


        public override string[] Textures { get; } = new[] {"greystone"};

        public override IMaterialDefinition Material { get; }
    }
}