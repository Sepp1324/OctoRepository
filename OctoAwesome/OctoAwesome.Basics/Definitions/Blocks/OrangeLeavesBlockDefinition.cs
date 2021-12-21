using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class OrangeLeavesBlockDefinition : BlockDefinition
    {
        public OrangeLeavesBlockDefinition(LeaveMaterialDefinition material) => Material = material;

        public override string Name => OctoBasics.OrangeLeaves;

        public override string Icon => "leaves_orange";

        public override string[] Textures { get; } = { "leaves_orange" };

        public override IMaterialDefinition Material { get; }
    }
}