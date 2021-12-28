using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Definition for Clay
    /// </summary>
    public class ClayMaterialDefinition : ISolidMaterialDefinition
    {
        /// <summary>
        /// Hardness of Clay
        /// </summary>
        public int Hardness => 3;

        /// <summary>
        /// Density of Clay
        /// </summary>
        public int Density => 2000;

        /// <summary>
        /// Granularity of Clay
        /// </summary>
        public int Granularity => 25;

        /// <summary>
        /// FractureToughness of Clay
        /// </summary>
        public int FractureToughness => 60;

        /// <summary>
        /// Material-Name
        /// </summary>
        public string Name => "Clay";

        /// <summary>
        /// Material-Inventory Icon
        /// </summary>
        public string Icon => string.Empty;
    }
}