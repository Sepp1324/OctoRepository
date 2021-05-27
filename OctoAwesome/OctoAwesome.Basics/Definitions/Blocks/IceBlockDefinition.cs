using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class IceBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Ice; }
        }

        public override string Icon
        {
            get { return "ice"; }
        }


<<<<<<< HEAD
        public override string[] Textures =>
            new[] {
                "ice"
=======
        public override string[] Textures
        {
            get
            {
                return new[] {
                    "ice"
                };
            }
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 2f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
>>>>>>> feature/performance
            };

<<<<<<< HEAD
        public override IMaterialDefinition Material { get; }

        public IceBlockDefinition(IceMaterialDefinition material) => Material = material;
=======
      
>>>>>>> feature/performance
    }
}
