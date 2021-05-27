using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class WaterBlockDefinition : BlockDefinition
    {
        public WaterBlockDefinition(WaterMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => OctoBasics.Water;

        public override uint SolidWall => 0;

        public override string Icon => "water";

        public override IMaterialDefinition Material { get; }


        public override string[] Textures { get; } = {"water"};
    }
}