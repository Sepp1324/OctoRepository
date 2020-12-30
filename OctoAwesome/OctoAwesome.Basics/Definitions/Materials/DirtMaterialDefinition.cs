using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class DirtMaterialDefinition : ISolidMaterialDefinition
    {
        public string Name => "Dirt";

        public string Icon => string.Empty;

        public int Hardness => 10;

        public int Density => 1400;
        
        public int Granularity { get; }
        
        public int FractureToughness { get; }
    }
}