using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class PlankBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Plank;

        public override string Icon => "planks_red";

        public override bool HasMetaData { get { return true; } }

        public override string[] Textures
        {
            get
            {
                return new[] {
                "planks_red"};
            }
        }

        public override IMaterialDefinition GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new IMaterialDefinition()
            {
                Density = 0.87f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }
    }
}
