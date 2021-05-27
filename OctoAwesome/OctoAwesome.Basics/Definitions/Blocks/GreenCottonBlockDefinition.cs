using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GreenCottonBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.GreenCotton; }
        }

        public override string Icon
        {
            get { return "cotton_green"; }
        }


<<<<<<< HEAD
        public override string[] Textures =>
            new[] {
                "cotton_green"
=======
        public override string[] Textures
        {
            get
            {
                return new[] {
                    "cotton_green"
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

        public GreenCottonBlockDefinition(CottonMaterialDefinition material) => Material = material;
=======
      
>>>>>>> feature/performance
    }
}
