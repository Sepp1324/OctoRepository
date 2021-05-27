using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GreystoneBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Greystone; }
        }

        public override string Icon
        {
            get { return "greystone"; }
        }


<<<<<<< HEAD
        public override string[] Textures =>
            new[] {
                "greystone",
=======
        public override string[] Textures
        {
            get
            {
                return new[] {
                    "greystone",
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

        public GreystoneBlockDefinition(StoneMaterialDefinition material) => Material = material;
=======
     
>>>>>>> feature/performance
    }
}
