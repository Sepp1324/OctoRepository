using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GrassBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Grass; }
        }

        public override string Icon
        {
            get { return "grass_top"; }
        }

<<<<<<< HEAD
        public override string[] Textures =>
            new[] {
                "grass_top",
                "dirt",
                "dirt_grass",
=======
        public override string[] Textures
        {
            get
            {
                

                return new[] {
                    "grass_top",
                    "dirt",
                    "dirt_grass",
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

        public GrassBlockDefinition(DirtMaterialDefinition material) => Material = material;

        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            switch (wall)
=======
      

        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            if (wall == Wall.Top)
            {
                return 0;
            } else if (wall == Wall.Bottom)
            {
                return 1;
            }
            else
>>>>>>> feature/performance
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
