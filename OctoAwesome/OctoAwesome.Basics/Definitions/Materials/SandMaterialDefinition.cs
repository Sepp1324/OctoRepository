using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class SandMaterialDefinition : IMaterialDefinition
    {
        public string Name => "Sand";

        public string Icon => string.Empty;

        public int Hardness => 70;

        public int Density => 1600;
        
        public int Granularity { get; }
        
        public int FractureToughness { get; }
    }
}