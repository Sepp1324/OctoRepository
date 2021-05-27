using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class OrangeLeavesBlockDefinition : BlockDefinition
    {
        public OrangeLeavesBlockDefinition(LeaveMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => Languages.OctoBasics.OrangeLeaves;

        public override string Icon => "leaves_orange";


        public override string[] Textures { get; } = new[] {"leaves_orange"};

        public override IMaterialDefinition Material { get; }
    }
}