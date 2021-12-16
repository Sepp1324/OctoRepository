using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class IceBlockDefinition : BlockDefinition
    {
        public IceBlockDefinition(IceMaterialDefinition material)
        {
            Material = material;
        }

        public override string Name => OctoBasics.Ice;

        public override string Icon => "ice";


        public override string[] Textures { get; } = { "ice" };

        public override IMaterialDefinition Material { get; }
    }
}