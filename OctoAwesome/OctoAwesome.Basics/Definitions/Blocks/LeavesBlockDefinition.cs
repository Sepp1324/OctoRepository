using System;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class LeavesBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Leaves;

        public override string Icon => "leaves";


        public override string[] Textures { get; } =
        {
            "leaves"
        };

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 2f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties) => throw new NotImplementedException();
    }
}