using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class BlueCottonBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.BlueCotton; }
        }

        public override string Icon
        {
            get { return "cotton_blue"; }
        }


<<<<<<< HEAD

        public override string[] Textures =>
            new[] {
                "cotton_blue"
            };
=======
        public override string[] Textures
        {
            get
            {
                return new[] {
                    "cotton_blue"
                };
            }
        }
>>>>>>> feature/performance

        public override IMaterialDefinition Material { get; }

<<<<<<< HEAD
        public BlueCottonBlockDefinition(CottonMaterialDefinition material) => Material = material;
=======
>>>>>>> feature/performance
    }
}
