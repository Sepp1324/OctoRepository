using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class GlassMaterialDefinition : ISolidMaterialDefinition
    {
        public string Name => "Glass";

        public string Icon => string.Empty;

        public int Hardness => 55;

        public int Density => 2500;
        
        public int Granularity { get; }
        
        public int FractureToughness { get; }
    }
}