using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class GravelMaterialDefinition : ISolidMaterialDefinition
    {
        public string Name => "Gravel";

        public string Icon => string.Empty;

        public int Hardness => 60;

        public int Density => 1440;
        
        public int Granularity { get; }
        
        public int FractureToughness { get; }
    }
}