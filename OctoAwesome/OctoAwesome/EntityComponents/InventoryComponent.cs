using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Serialization;

namespace OctoAwesome.EntityComponents
{
    public class InventoryComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {
        private readonly IDefinitionManager _definitionManager;

        public InventoryComponent()
        {
            Inventory = new();
            _definitionManager = TypeContainer.Get<IDefinitionManager>();
        }

        /// <summary>
        ///     Das Inventar der Entity
        /// </summary>
        public List<InventorySlot> Inventory { get; set; }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var name = reader.ReadString();

                var definition = _definitionManager.Definitions.FirstOrDefault(d => d.GetType().FullName == name);

                decimal amount = 1;
                IInventoryable inventoryItem = default;
                if (definition is IInventoryable inventoryable)
                {
                    amount = reader.ReadDecimal();
                    inventoryItem = inventoryable;
                }
                else
                {
                    var type = Type.GetType(name);

                    if (type is null)
                        continue;

                    object instance;
                    if (type.IsAssignableTo(typeof(Item)))
                    {
                        instance = Item.Deserialize(reader, type, _definitionManager);
                    }
                    else
                    {
                        instance = Activator.CreateInstance(type)!;
                        if (instance is ISerializable serializable) serializable.Deserialize(reader);
                    }


                    if (instance is IInventoryable inventoryObject) inventoryItem = inventoryObject;
                }

                if (inventoryItem == default)
                    continue;

                var slot = new InventorySlot
                {
                    Amount = amount,
                    Item = inventoryItem
                };

                Inventory.Add(slot);
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Inventory.Count);
            foreach (var slot in Inventory)
                switch (slot.Item)
                {
                    case Item item:
                        writer.Write(slot.Item.GetType().AssemblyQualifiedName!);
                        Item.Serialize(writer, item);
                        break;
                    case ISerializable serializable:
                        writer.Write(slot.Item.GetType().AssemblyQualifiedName!);
                        serializable.Serialize(writer);
                        break;
                    default:
                        writer.Write(slot.Item.GetType().FullName!);
                        writer.Write(slot.Amount);
                        break;
                }
        }

        /// <summary>
        ///     Fügt ein Element des angegebenen Definitionstyps hinzu.
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="item">Die Definition.</param>
        public void AddUnit(int quantity, IInventoryable item)
        {
            var slot = Inventory.FirstOrDefault(s => s.Item == item && s.Amount < item.VolumePerUnit * item.StackLimit);

            // Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
            if (slot == null)
            {
                slot = new()
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
            if (slot.Item is not IInventoryable definition)
                return false;

            if (slot.Amount < definition.VolumePerUnit)
                return false;

            slot.Amount -= definition.VolumePerUnit;

            return slot.Amount > 0 || Inventory.Remove(slot);
        }

        public bool RemoveSlot(InventorySlot inventorySlot) => Inventory.Remove(inventorySlot);

        public void AddSlot(InventorySlot inventorySlot)
        {
            var slot = Inventory.FirstOrDefault(s => s.Item == inventorySlot.Item &&
                                                     s.Amount < s.Item.VolumePerUnit * s.Item.StackLimit);

            // Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
            if (slot == null)
            {
                slot = new()
                {
                    Item = inventorySlot.Item,
                    Amount = inventorySlot.Amount
                };
                Inventory.Add(slot);
            }
            else
            {
                slot.Amount += inventorySlot.Amount;
            }
        }
    }
}