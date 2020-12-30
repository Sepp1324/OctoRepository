using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class ClayMaterialDefinition : ISolidMaterialDefinition
    {
        public string Name => "Clay";

        public string Icon => string.Empty;

        public int Hardness => 3;

        public int Density => 2000;
        
        public int Granularity { get; }
        
        public int FractureToughness { get; }
    }
}