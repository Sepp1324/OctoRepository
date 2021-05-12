using OctoAwesome.Definitions;

namespace OctoAwesome
{
    /// <summary>
    /// Ein Slot in einem Inventar
    /// </summary>
    public class InventorySlot
    {
        private IInventoryable item;
        
        /// <summary>
        /// Das Item das in dem Slot ist.
        /// </summary>
        public IInventoryable Item
        {
            get => item;
            set
            {
                switch (value)
                {
                    case IDefinition definition:
                        Definition = definition;
                        break;
                    case IItem item:
                        Definition = item.Definition;
                        break;
                    default:
                        Definition = null;
                        break;
                }

                item = value;
            }
        }

        /// <summary>
        /// Volumen des Elementes <see cref="Item"/> in diesem Slot in dm³.
        /// </summary>
        public decimal Amount { get; set; }
        
        public IDefinition Definition { get; set; }
    }
}
