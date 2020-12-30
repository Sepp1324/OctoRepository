using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class CottonMaterialDefinition : ISolidMaterialDefinition
    {
        public string Name => "Cotton";

        public string Icon => string.Empty;

        public int Hardness => 4;

        public int Density => 132;
        
        public int Granularity { get; }
        
        public int FractureToughness { get; }
    }
}