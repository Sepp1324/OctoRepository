using System;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class RedPlankBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.RedPlank;

        public override string Icon => "planks";

        public override bool HasMetaData => true;

        public override string[] Textures { get; } =
        {
            "planks"
        };

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 0.87f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties) => throw new NotImplementedException();
    }
}