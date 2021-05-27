using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class LeavesBlockDefinition : BlockDefinition
    {
        public LeavesBlockDefinition(LeaveMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => Languages.OctoBasics.Leaves;

        public override string Icon => "leaves";


        public override string[] Textures { get; } = new[] {"leaves"};

        public override IMaterialDefinition Material { get; }
    }
}