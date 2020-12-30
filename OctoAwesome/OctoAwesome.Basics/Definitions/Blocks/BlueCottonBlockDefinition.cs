using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class BlueCottonBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.BlueCotton;

        public override string Icon => "cotton_blue";


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "cotton_blue"
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
