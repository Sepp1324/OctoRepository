using System;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class PlankBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Plank;

        public override string Icon => "planks_red";

        public override bool HasMetaData { get { return true; } }

        public override string[] Textures => new[] {
                "planks_red"};

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z) => new PhysicalProperties()
        {
            Density = 0.87f,
            FractureToughness = 0.3f,
            Granularity = 0.9f,
            Hardness = 0.1f
        };

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties) => throw new NotImplementedException();

    }
}
