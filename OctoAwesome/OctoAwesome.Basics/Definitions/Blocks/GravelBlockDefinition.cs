using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GravelBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Gravel; }
        }

        public override string Icon
        {
            get { return "gravel"; }
        }

<<<<<<< HEAD
        public override string[] Textures =>
            new[] {
                "gravel",
=======

        public override string[] Textures
        {
            get
            {
                return new[] {
                    "gravel",
                };
            }
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 2.5f,
                FractureToughness = 0.1f,
                Granularity = 0.1f,
                Hardness = 0.9f
>>>>>>> feature/performance
            };

<<<<<<< HEAD
        public override IMaterialDefinition Material { get; }

        public GravelBlockDefinition(GravelMaterialDefinition material) => Material = material;
=======
        
>>>>>>> feature/performance
    }
}
