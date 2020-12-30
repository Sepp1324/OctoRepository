using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GravelBLockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Gravel;

        public override string Icon => "gravel";


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "gravel",
                };
            }
        }

        public override IMaterialDefinition GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new IMaterialDefinition()
            {
                Density = 2.5f,
                FractureToughness = 0.1f,
                Granularity = 0.1f,
                Hardness = 0.9f
            };
        }
    }
}
