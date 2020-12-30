using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class DirtBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Ground;

        public override string Icon => "dirt";


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "dirt"
                };
            }
        }

        public override IMaterialDefinition GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new IMaterialDefinition()
            {
                Density = 2f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }
    }
}
