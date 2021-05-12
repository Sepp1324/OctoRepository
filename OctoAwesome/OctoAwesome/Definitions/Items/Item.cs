namespace OctoAwesome.Definitions.Items
{
    /// <summary>
    /// Basisklasse für alle nicht-lebendigen Spielelemente (für lebendige Spielelemente siehe <see cref="Entity"/>
    /// </summary>
    public abstract class Item : IItem
    {
        /// <summary>
        /// Der Zustand des Items
        /// </summary>
        public int Condition { get; set; }

        /// <summary>
        /// Die Koordinate, an der das Item in der Welt herumliegt, falls es nicht im Inventar ist
        /// </summary>
        public Coordinate? Position { get; set; }

        public IItemDefinition Definition { get; }
        
        public IMaterialDefinition Material { get; set; }

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse Item.
        /// </summary>
        public Item(IItemDefinition definition)
        {
            Definition = definition;
            Condition = 99;
        }
        
        public virtual int Hit(IMaterialDefinition material, decimal blockVolumeVolumeRemaining, int volumePerHit)
        {
            if (!Definition.CanMineMaterial(material))
                return 0;

            if (material.Density > 1)
                return volumePerHit;

            if (Material.Hardness * 1.2f < material.Hardness)
                return 0;

            return ((Material.Hardness - material.Hardness) * 3 + 100) * volumePerHit / 100;
        }
    }
}
