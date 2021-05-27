using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class TanCottonBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.TanCotton; }
        }

        public override string Icon
        {
            get { return "cotton_tan"; }
        }


<<<<<<< HEAD
        public override string[] Textures =>
            new[] {
                "cotton_tan"
            };

        public override IMaterialDefinition Material { get; }

        public TanCottonBlockDefinition(CottonMaterialDefinition material) => Material = material;
=======
        public override string[] Textures
        {
            get
            {
                return new[] {
                    "cotton_tan"
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
            };
        }
     
>>>>>>> feature/performance
    }
}
