using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Definition for Dirt
    /// </summary>
    public sealed class DirtBlockDefinition : BlockDefinition
    {
        /// <summary>
        /// Definition for Dirt
        /// </summary>
        /// <param name="material"><see cref="DirtMaterialDefinition"/></param>
        public DirtBlockDefinition(DirtMaterialDefinition material) => Material = material;

        /// <summary>
        /// Material-Name
        /// </summary>
        public override string Name => OctoBasics.Ground;

        /// <summary>
        /// Material-Inventory Icon
        /// </summary>
        public override string Icon => "dirt";

        /// <summary>
        /// Material-Textures
        /// </summary>
        public override string[] Textures { get; } = { "dirt" };

        /// <summary>
        /// Material
        /// </summary>
        public override IMaterialDefinition Material { get; }
    }
}