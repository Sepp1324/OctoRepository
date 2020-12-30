using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class IceBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Ice;

        public override string Icon => "ice";


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "ice"
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
