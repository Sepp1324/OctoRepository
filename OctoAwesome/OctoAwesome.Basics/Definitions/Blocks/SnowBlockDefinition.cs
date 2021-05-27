using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public class SnowBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get
            {
                return Languages.OctoBasics.Snow;
            }
        }

        public override string Icon
        {
            get
            {
                return "snow"; 
            }
        }

<<<<<<< HEAD
        public override string[] Textures =>
            new[] {
                "snow",
                "dirt",
                "dirt_snow",
            };

        public override IMaterialDefinition Material { get; }

        public SnowBlockDefinition(SnowMaterialDefinition material) => Material = material;

=======
        public override string[] Textures
        {
            get
            {
                return new[] {
                    "snow",
                    "dirt",
                    "dirt_snow",
                };
            }
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 1.5f,
                FractureToughness = 0.2f,
                Granularity = 0.9f,
                Hardness = 0.05f
            };
        }
             
>>>>>>> feature/performance
        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            switch (wall)
            {
                case Wall.Top:
                    return 0;
                case Wall.Bottom:
                    return 1;
                default:
                    return 2;
            }
        }
    }
}
