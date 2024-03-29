﻿using System;
using OctoAwesome.Components;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    ///     EntityComponent, die eine Werkzeug-Toolbar für den Apieler bereitstellt.
    /// </summary>
    public class ToolBarComponent : Component, IEntityComponent
    {
        /// <summary>
        ///     Gibt die Anzahl Tools in der Toolbar an.
        /// </summary>
        public const int TOOL_COUNT = 10;

        private int _activeIndex;


        /// <summary>
        ///     Erzeugte eine neue ToolBarComponent
        /// </summary>
        public ToolBarComponent()
        {
            HandSlot = new() { Item = new Hand(new()) };
            Tools = new InventorySlot[TOOL_COUNT];
            ActiveIndex = 0;
        }

        /// <summary>
        ///     Auflistung der Werkzeuge die der Spieler in seiner Toolbar hat.
        /// </summary>
        public InventorySlot[] Tools { get; set; }

        /// <summary>
        ///     Derzeit aktives Werkzeug des Spielers
        /// </summary>
        public InventorySlot ActiveTool => Tools[_activeIndex] ?? HandSlot;

        /// <summary>
        /// </summary>
        public InventorySlot HandSlot { get; }

        /// <summary>
        /// </summary>
        public int ActiveIndex
        {
            get => _activeIndex;
            set => _activeIndex = (value + TOOL_COUNT) % TOOL_COUNT;
        }

        /// <summary>
        /// </summary>
        public event Action<InventorySlot, int> OnChanged;

        /// <summary>
        ///     Entfernt einen InventorySlot aus der Toolbar
        /// </summary>
        /// <param name="slot"></param>
        public void RemoveSlot(InventorySlot slot)
        {
            for (var i = 0; i < Tools.Length; i++)
                if (Tools[i] == slot)
                {
                    Tools[i] = null;
                    OnChanged?.Invoke(HandSlot, i);
                    break;
                }
        }

        /// <summary>
        ///     Setzt einen InventorySlot an eine Stelle in der Toolbar und löscht ggf. vorher den Slot aus alten Positionen.
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
        ///     Gibt den Index eines InventorySlots in der Toolbar zurück.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns>Den Index des Slots, falls nicht gefunden -1.</returns>
        public int GetSlotIndex(InventorySlot slot)
        {
            for (var j = 0; j < Tools.Length; j++)
                if (Tools[j] == slot)
                    return j;

            return -1;
        }

        /// <summary>
        ///     Fügt einen neuen InventorySlot an der ersten freien Stelle hinzu.
        /// </summary>
        /// <param name="slot"></param>
        public void AddNewSlot(InventorySlot slot)
        {
            for (var i = 0; i < Tools.Length; i++)
                if (Tools[i] == null)
                {
                    Tools[i] = slot;
                    OnChanged?.Invoke(slot, i);
                    break;
                }
        }
    }
}