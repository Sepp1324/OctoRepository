namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Interface, das ein Item darstellt
    /// </summary>
    public interface IItem
    {

        /// <summary>
        /// Die Koordinate, an der das Item in der Welt herumliegt, falls es nicht im Inventar ist
        /// </summary>
        Coordinate? Position { get; set; }

        /// <summary>
        /// Der Zustand des Items
        /// </summary>
        int Condition { get; set; }
<<<<<<< HEAD:OctoAwesome/OctoAwesome/Definitions/IItem.cs
        
        IItemDefinition Definition { get; }
        
        IMaterialDefinition Material { get; set; }
        
        int Hit(IMaterialDefinition material, decimal blockVolumeVolumeRemaining, int volumePerHit);
=======
        IItemDefinition Definition { get; }
>>>>>>> feature/performance:OctoAwesome/OctoAwesome/IItem.cs
    }
}
