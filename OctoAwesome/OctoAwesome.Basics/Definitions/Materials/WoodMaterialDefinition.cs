using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class WoodMaterialDefinition : ISolidMaterialDefinition
    {
        public string Name => "Wood";

        public string Icon => string.Empty;

        public int Hardness => 35; //Mohs Hardness Scala

        public int Density => 680; // k/m^3
        
        
        public int Granularity { get; }
        
        public int FractureToughness { get; }
    }
}