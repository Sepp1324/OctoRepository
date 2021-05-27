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
<<<<<<< HEAD
        /// Stores all of the Tools of a Player
        /// </summary>
        public InventorySlot[] Tools { get; set; }

        /// <summary>
        /// Active Tool of the Player
=======
        /// Auflistung der Werkzeuge die der Spieler in seiner Toolbar hat.
>>>>>>> feature/performance
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
<<<<<<< HEAD
        /// Removes an InventorySlot from the Toolbar
=======
        /// Erzeugte eine neue ToolBarComponent
        /// </summary>
        public ToolBarComponent()
        {
            Tools = new InventorySlot[TOOLCOUNT];
        }

        /// <summary>
        /// Entfernt einen InventorySlot aus der Toolbar
>>>>>>> feature/performance
        /// </summary>
        /// <param name="slot"></param>
        public void RemoveSlot(InventorySlot slot)
        {
            for (int i = 0; i < Tools.Length; i++)
            {
                if (Tools[i] == slot)
                {
                    Tools[i] = null;
                    OnChanged?.Invoke(HandSlot, i);
                    break;
                }
            }
<<<<<<< HEAD
=======
            if (ActiveTool == slot)
                ActiveTool = null;
>>>>>>> feature/performance
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
            for (int j = 0; j < Tools.Length; j++)
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
            for (int i = 0; i < Tools.Length; i++)
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
