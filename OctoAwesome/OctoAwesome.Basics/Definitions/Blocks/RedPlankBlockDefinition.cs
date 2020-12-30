using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class RedPlankBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.RedPlank;

        public override string Icon => "planks";

        public override bool HasMetaData { get { return true; } }

        public override string[] Textures
        {
            get
            {
                return new[] {
                "planks"};
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
