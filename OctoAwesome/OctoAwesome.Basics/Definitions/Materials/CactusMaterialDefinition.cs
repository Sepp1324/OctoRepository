using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class CactusMaterialDefinition : ISolidMaterialDefinition
    {
        public string Name => "Cactus";

        public string Icon => string.Empty;

        public int Hardness => 25;

        public int Density => 850;
        
        public int Granularity { get; }
        
        public int FractureToughness { get; }
    }
}