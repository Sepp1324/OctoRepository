using System;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class BlueCottonBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.BlueCotton;

        public override string Icon => "cotton_blue";

        public override string[] Textures => new[] { "cotton_blue" };

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z) => new PhysicalProperties()
        {
            Density = 2f,
            FractureToughness = 0.3f,
            Granularity = 0.9f,
            Hardness = 0.1f
        };

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties) => throw new NotImplementedException();
    }
}
