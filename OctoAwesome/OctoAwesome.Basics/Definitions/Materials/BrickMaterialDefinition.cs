using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class BrickMaterialDefinition : ISolidMaterialDefinition
    {
        public string Name => "Brick";

        public string Icon => string.Empty;

        public int Hardness => 45;

        public int Density => 1800;
        
        public int Granularity { get; }
        
        public int FractureToughness { get; }
    }
}