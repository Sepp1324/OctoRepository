using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Definition for Cotton
    /// </summary>
    public class CottonMaterialDefinition : ISolidMaterialDefinition
    {
        /// <summary>
        /// Hardness of Cotton
        /// </summary>
        public int Hardness => 4;

        /// <summary>
        /// Density of Cotton
        /// </summary>
        public int Density => 132;

        /// <summary>
        /// Granularity of Cotton
        /// </summary>
        public int Granularity => 10;

        /// <summary>
        /// FractureToughness of Cotton
        /// </summary>
        public int FractureToughness => 600;

        /// <summary>
        /// Material-Name
        /// </summary>
        public string Name => "Cotton";

        /// <summary>
        /// Material-Inventory Icon
        /// </summary>
        public string Icon => string.Empty;
    }
}