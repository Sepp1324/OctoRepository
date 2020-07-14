using System;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class SandBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Sand;

        public override string Icon => "sand";

        public override string[] Textures => new[] {
                    "sand"
                };

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties) => throw new NotImplementedException();

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z) => new PhysicalProperties()
        {
            //Schüttdichte
            Density = 1.5f,
            FractureToughness = 0.3f,
            Granularity = 0.9f,
            Hardness = 0.1f
        };
    }
}
