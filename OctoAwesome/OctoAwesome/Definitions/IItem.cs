﻿namespace OctoAwesome.Definitions
{
    /// <summary>
    ///     Interface, das ein Item darstellt
    /// </summary>
    public interface IItem
    {
        /// <summary>
        ///     Die Koordinate, an der das Item in der Welt herumliegt, falls es nicht im Inventar ist
        /// </summary>
        Coordinate? Position { get; set; }

        /// <summary>
        ///     Der Zustand des Items
        /// </summary>
        int Condition { get; set; }

        /// <summary>
        /// </summary>
        IItemDefinition Definition { get; }

        /// <summary>
        /// </summary>
        IMaterialDefinition Material { get; }

        int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining, int volumePerHit);
    }
}