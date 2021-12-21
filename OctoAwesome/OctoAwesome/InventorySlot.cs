using OctoAwesome.Definitions;

namespace OctoAwesome
{
    /// <summary>
    ///     Ein Slot in einem Inventar
    /// </summary>
    public class InventorySlot
    {
        private IInventoryable _item;

        /// <summary>
        ///     Das Item das in dem Slot ist.
        /// </summary>
        public IInventoryable Item
        {
            get => _item;
            set
            {
                Definition = value switch
                {
                    IDefinition definition => definition,
                    IItem item => item.Definition,
                    _ => null
                };

                _item = value;
            }
        }

        /// <summary>
        ///     Volumen des Elementes <see cref="Item" /> in diesem Slot in dm³.
        /// </summary>
        public decimal Amount { get; set; }

        public IDefinition Definition { get; set; }
    }
}