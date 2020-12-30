using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class SandBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Sand;

        public override string Icon => "sand";


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "sand"
                };
            }
        }

        public override IMaterialDefinition GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new IMaterialDefinition()
            {
                //Schüttdichte
                Density = 1.5f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }
    }
}
