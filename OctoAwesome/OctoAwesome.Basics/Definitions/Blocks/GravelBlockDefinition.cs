using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Definition for Gravel
    /// </summary>
    public sealed class GravelBlockDefinition : BlockDefinition
    {
        /// <summary>
        /// Definition for Gravel
        /// </summary>
        /// <param name="material"><see cref="GravelMaterialDefinition"/></param>
        public GravelBlockDefinition(GravelMaterialDefinition material) => Material = material;

        /// <summary>
        /// Block-Name
        /// </summary>
        public override string Name => OctoBasics.Gravel;

        /// <summary>
        /// Block-Inventory-Item
        /// </summary>
        public override string Icon => "gravel";

        /// <summary>
        /// Block-Textures
        /// </summary>
        public override string[] Textures { get; } = { "gravel" };

        /// <summary>
        /// Block-Material
        /// </summary>
        public override IMaterialDefinition Material { get; }
    }
}