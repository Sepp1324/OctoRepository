using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class StoneBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Stone;

        public override string Icon => "stone";


        public override string[] Textures
        {
            get
            {
                return new[]
                {
                    "stone",
                };
            }
        }

        public override IMaterialDefinition GetProperties(ILocalChunkCache manager, int x, int y, int z) =>
            new IMaterialDefinition
            {
                Density = 2.5f,
                FractureToughness = 0.1f,
                Granularity = 0.1f,
                Hardness = 0.9f
            };
    }
}