using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class RedPlankBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.RedPlank; }
        }

        public override string Icon
        {
            get { return "planks"; }
        }

        public override bool HasMetaData { get { return true; } }

<<<<<<< HEAD
        public override string[] Textures =>
            new[] {
                "planks"};
=======
        public override string[] Textures
        {
            get
            {
                return new[] {
                "planks"};
            }
        }
>>>>>>> feature/performance

        public override IMaterialDefinition Material { get; }

<<<<<<< HEAD
        public RedPlankBlockDefinition(WoodMaterialDefinition material) => Material = material;
=======
       

>>>>>>> feature/performance
    }
}
