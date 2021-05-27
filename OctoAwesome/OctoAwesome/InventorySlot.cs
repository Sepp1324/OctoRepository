using OctoAwesome.Definitions;

namespace OctoAwesome
{
    /// <summary>
    ///     Ein Slot in einem Inventar
    /// </summary>
    public class InventorySlot
    {
        private IInventoryable item;

        /// <summary>
        ///     Das Item das in dem Slot ist.
        /// </summary>
        public IInventoryable Item
        {
            get => item;
            set
            {
                if (value is IDefinition definition)
                    Definition = definition;
                else if (value is IItem item)
                    Definition = item.Definition;
                else
                    Definition = null;

                item = value;
            }
        }

        /// <summary>
        ///     Volumen des Elementes <see cref="Item" /> in diesem Slot in dm³.
        /// </summary>
        public decimal Amount { get; set; }

        public IDefinition Definition { get; set; }
    }
}