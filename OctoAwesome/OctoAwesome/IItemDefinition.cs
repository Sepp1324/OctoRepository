using System.Drawing;

namespace OctoAwesome
{
    /// <summary>
    /// Interface für die Definition eînes Items
    /// </summary>
    public interface IItemDefinition
    {
        /// <summary>
        /// Der Name des Items
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Bild, das das Item repräsentiert
        /// </summary>
        Bitmap Icon { get; }

        /// <summary>
        /// Gibt an, wie viele dieses Items im Inventar in einem Slot gestapelt werden können
        /// </summary>
        int StackLimit { get; }
    }
}
