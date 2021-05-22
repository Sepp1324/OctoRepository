using System;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Provides the Toolbar for a Player
    /// </summary>
    public class ToolBarComponent : EntityComponent
    {
        private int _activeIndex;
        
        /// <summary>
        /// Count of the Tools in the Toolbar
        /// </summary>
        public const int Toolcount = 10;

        /// <summary>
        /// Stores all of the Tools of a Player
        /// </summary>
        public InventorySlot[] Tools { get; set; }

        /// <summary>
        /// Active Tool of the Player
        /// </summary>
        public InventorySlot ActiveTool => Tools[_activeIndex] ?? HandSlot;
        
        public InventorySlot HandSlot { get; }
        
        /// <summary>
        /// Represents the Index of the selected Item
        /// </summary>
        public int ActiveIndex { get => _activeIndex; set => _activeIndex = (value + Toolcount) % Toolcount; }

        public event Action<InventorySlot, int> OnChanged;

        /// <summary>
        /// Creates a new ToolBarComponent Instance
        /// </summary>
        public ToolBarComponent()
        {
            HandSlot = new InventorySlot {Item = new Hand(new HandDefinition())}; 
            Tools = new InventorySlot[Toolcount];
            ActiveIndex = 0;
        }

        /// <summary>
        /// Removes an InventorySlot from the Toolbar
        /// </summary>
        /// <param name="slot"></param>
        public void RemoveSlot(InventorySlot slot)
        {
            for (var i = 0; i < Tools.Length; i++)
            {
                if (Tools[i] == slot)
                {
                    Tools[i] = null;
                    OnChanged?.Invoke(HandSlot, i);
                    break;
                }
            }
        }

        /// <summary>
        /// Sets a Tool to a certain Slot
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="index"></param>
        public void SetTool(InventorySlot slot, int index)
        {
            RemoveSlot(slot);

            Tools[index] = slot;
            OnChanged?.Invoke(slot, index);
        }

        /// <summary>
        /// Returns the Index of an InventorySlot
        /// </summary>
        /// <param name="slot"></param>
        /// <returns>Index of InventorySlot; 404: -1</returns>
        public int GetSlotIndex(InventorySlot slot)
        {
            for (var j = 0; j < Tools.Length; j++)
                if (Tools[j] == slot)
                    return j;

            return -1;
        }

        /// <summary>
        /// Adds a new InventorySlot to the first possible place
        /// </summary>
        /// <param name="slot"></param>
        public void AddNewSlot(InventorySlot slot)
        {
            for (var i = 0; i < Tools.Length; i++)
            {
                if (Tools[i] == null)
                {
                    Tools[i] = slot;
                    OnChanged?.Invoke(slot, i);
                    break;
                }
            }
        }
    }
}
