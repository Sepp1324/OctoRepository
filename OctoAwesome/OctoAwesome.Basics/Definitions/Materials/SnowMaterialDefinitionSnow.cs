using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class SnowMaterialDefinition : ISolidMaterialDefinition
    {
        public string Name => "Snow";

        public string Icon => string.Empty;

        public int Hardness => 1;

        public int Density => 250;
        
        public int Granularity { get; }
        
        public int FractureToughness { get; }
    }
}