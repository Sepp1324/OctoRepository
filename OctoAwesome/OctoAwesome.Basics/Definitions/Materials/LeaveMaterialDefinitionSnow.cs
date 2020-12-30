using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class LeaveMaterialDefinition : ISolidMaterialDefinition
    {
        public string Name => "Leave";

        public string Icon => string.Empty;
        
        public int Hardness => 1;

        public int Density => 200;
        
        public int Granularity { get; }
        
        public int FractureToughness { get; }
    }
}