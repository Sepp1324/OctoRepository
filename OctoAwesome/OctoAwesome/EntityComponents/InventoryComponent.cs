using OctoAwesome.Definitions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.EntityComponents
{
    public class InventoryComponent : EntityComponent
    {
        /// <summary>
        /// Das Inventar der Entity
        /// </summary>
        public List<InventorySlot> Inventory { get; set; }

<<<<<<< HEAD
        public InventoryComponent() => Inventory = new List<InventorySlot>();
=======
        public InventoryComponent()
        {
            Inventory = new List<InventorySlot>();
        }
>>>>>>> feature/performance

        public override void Deserialize(BinaryReader reader)
        {
            if (!TypeContainer.TryResolve(out IDefinitionManager definitionManager))
                return;

            base.Deserialize(reader);

            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
<<<<<<< HEAD
                var name = reader.ReadString();
                var definition = definitionManager.Definitions.FirstOrDefault(d => d.GetType().FullName == name);
=======
                string name = reader.ReadString();
                var definition = definitionManager.GetDefinitions().FirstOrDefault(d => d.GetType().FullName == name);
>>>>>>> feature/performance
                var amount = reader.ReadDecimal();

                if (definition == null || !(definition is IInventoryable))
                    continue;

                var slot = new InventorySlot()
                {
                    Amount = amount,
<<<<<<< HEAD
                    Item = (IInventoryable)definition,
=======
                    Definition = (IInventoryableDefinition)definition,
>>>>>>> feature/performance
                };

                Inventory.Add(slot);
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Inventory.Count);
            foreach (var slot in Inventory)
            {
                writer.Write(slot.Item.GetType().FullName);
                writer.Write(slot.Amount);
            }
        }

        /// <summary>
        /// Fügt ein Element des angegebenen Definitionstyps hinzu.
        /// </summary>
<<<<<<< HEAD
        /// <param name="item">Die Definition.</param>
        public void AddUnit(int quantity, IInventoryable item)
        {
            var slot = Inventory.FirstOrDefault(s => s.Item == item &&
                s.Amount < item.VolumePerUnit * item.StackLimit);
=======
        /// <param name="definition">Die Definition.</param>
        public void AddUnit(int quantity, IInventoryableDefinition definition)
        {
            var slot = Inventory.FirstOrDefault(s => s.Definition == definition &&
                s.Amount < definition.VolumePerUnit * definition.StackLimit);
>>>>>>> feature/performance

            // Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
            if (slot == null)
            {
                slot = new InventorySlot()
                {
                    Item = item,
                    Amount = quantity
                };
                Inventory.Add(slot);
            }
<<<<<<< HEAD
            else
            {
                slot.Amount += quantity;
            }
=======
            slot.Amount += quantity;
>>>>>>> feature/performance
        }

        /// <summary>
        /// Entfernt eine Einheit vom angegebenen Slot.
        /// </summary>
        /// <param name="slot">Der Slot, aus dem entfernt werden soll.</param>
        /// <returns>Gibt an, ob das entfernen der Einheit aus dem Inventar funktioniert hat. False, z.B. wenn nicht genügend Volumen (weniger als VolumePerUnit) übrig ist-</returns>
        public bool RemoveUnit(InventorySlot slot)
        {
            if (!(slot.Item is IInventoryable definition))
                return false;

            if (slot.Amount >= definition.VolumePerUnit) // Wir können noch einen Block setzen
            {
                slot.Amount -= definition.VolumePerUnit;
                if (slot.Amount <= 0)
                    Inventory.Remove(slot);
                return true;
            }
            return false;
        }
    }
}
