using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class DirtBlockDefinition : BlockDefinition
    {
        public DirtBlockDefinition(DirtMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => Languages.OctoBasics.Ground;

        public override string Icon => "dirt";


        public override string[] Textures { get; } = new[] {"dirt"};


        public override IMaterialDefinition Material { get; }
    }
}