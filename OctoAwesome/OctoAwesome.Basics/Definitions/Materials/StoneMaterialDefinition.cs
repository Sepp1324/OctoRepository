using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class StoneMaterialDefinition : ISolidMaterialDefinition
    {
        public string Name => "Stone";

        public string Icon => string.Empty;

        public int Hardness => 60;

        public int Density => 2700;
        
        public int Granularity { get; }
        
        public int FractureToughness { get; }
    }
}