using System;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class WaterMaterialDefinition : IFluidMaterialDefinition
    {
        public string Name => "Water";

        public string Icon => string.Empty;

        public int Hardness => 0;

        public int Density => 997;

        public int FractureToughness { get; }
    }
}