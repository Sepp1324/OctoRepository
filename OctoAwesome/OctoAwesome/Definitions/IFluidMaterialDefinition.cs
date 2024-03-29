﻿namespace OctoAwesome.Definitions
{
    /// <summary>
    /// </summary>
    public interface IFluidMaterialDefinition : IMaterialDefinition
    {
        /// <summary>
        ///     Viscosity describes the tenacity of liquids
        ///     This value is in µPa·s
        /// </summary>
        int Viscosity { get; }
    }
}