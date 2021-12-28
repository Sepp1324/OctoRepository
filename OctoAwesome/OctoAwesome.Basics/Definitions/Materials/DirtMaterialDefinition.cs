using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Definition of Dirt
    /// </summary>
    public class DirtMaterialDefinition : ISolidMaterialDefinition
    {
        /// <summary>
        /// Hardness of Dirt
        /// </summary>
        public int Hardness => 10;

        /// <summary>
        /// Density of Dirt
        /// </summary>
        public int Density => 1400;

        /// <summary>
        /// Granularity of Dirt
        /// </summary>
        public int Granularity => 50;

        /// <summary>
        /// FractureToughness of Dirt
        /// </summary>
        public int FractureToughness => 50;

        /// <summary>
        /// Material-Name
        /// </summary>
        public string Name => "Dirt";

        /// <summary>
        /// Material-Inventory Icon
        /// </summary>
        public string Icon => string.Empty;
    }
}