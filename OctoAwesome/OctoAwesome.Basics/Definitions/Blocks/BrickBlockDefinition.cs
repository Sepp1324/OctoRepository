using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.Languages;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Definition for Brick
    /// </summary>
    public sealed class BrickBlockDefinition : BlockDefinition
    {
        /// <summary>
        /// Definition for Brick
        /// </summary>
        /// <param name="material"><see cref="BrickMaterialDefinition"/></param>
        public BrickBlockDefinition(BrickMaterialDefinition material) => Material = material;

        /// <summary>
        /// Material-Name
        /// </summary>
        public override string Name => OctoBasics.Brick;

        /// <summary>
        /// Material-Inventory Texture
        /// </summary>
        public override string Icon => "brick_red";

        /// <summary>
        /// Material-Texture
        /// </summary>
        public override string[] Textures { get; } = { "brick_red" };

        /// <summary>
        /// Material
        /// </summary>
        public override IMaterialDefinition Material { get; }
    }
}