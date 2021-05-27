using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctoAwesome.Definitions;

namespace OctoAwesome.EntityComponents
{
    public class InventoryComponent : EntityComponent
    {
        public InventoryComponent()
        {
            Inventory = new List<InventorySlot>();
        }

        /// <summary>
        ///     Das Inventar der Entity
        /// </summary>
        public List<InventorySlot> Inventory { get; set; }

        public override void Deserialize(BinaryReader reader)
        {
            if (!TypeContainer.TryResolve(out IDefinitionManager definitionManager))
                return;

            base.Deserialize(reader);

            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var name = reader.ReadString();
                var definition = definitionManager.Definitions.FirstOrDefault(d => d.GetType().FullName == name);
                var amount = reader.ReadDecimal();

                if (definition == null || !(definition is IInventoryable))
                    continue;

                var slot = new InventorySlot
                {
                    Amount = amount,
                    Item = (IInventoryable) definition
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
        ///     Fügt ein Element des angegebenen Definitionstyps hinzu.
        /// </summary>
        /// <param name="item">Die Definition.</param>
        public void AddUnit(int quantity, IInventoryable item)
        {
            var slot = Inventory.FirstOrDefault(s => s.Item == item &&
                                                     s.Amount < item.VolumePerUnit * item.StackLimit);

            // Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
            if (slot == null)
            {
                slot = new InventorySlot
                {
                    Item = item,
                    Amount = quantity
                };
                Inventory.Add(slot);
            }
            else
            {
                slot.Amount += quantity;
            }
        }

        /// <summary>
        ///     Entfernt eine Einheit vom angegebenen Slot.
        /// </summary>
        /// <param name="slot">Der Slot, aus dem entfernt werden soll.</param>
        /// <returns>
        ///     Gibt an, ob das entfernen der Einheit aus dem Inventar funktioniert hat. False, z.B. wenn nicht genügend
        ///     Volumen (weniger als VolumePerUnit) übrig ist-
        /// </returns>
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