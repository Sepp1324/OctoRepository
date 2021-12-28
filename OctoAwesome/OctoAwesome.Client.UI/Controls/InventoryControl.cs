using System;
using System.Collections.Generic;
using engenious;
using engenious.Graphics;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.UI.Components;

namespace OctoAwesome.UI.Controls
{
    public sealed class InventoryControl : Panel
    {
        private const int COLUMNS = 8;
        private readonly AssetComponent _assets;
        private readonly ScrollContainer _scroll;

        private Grid _grid;

        public InventoryControl(BaseScreenComponent manager, AssetComponent assets, List<InventorySlot> inventorySlots, int columns = COLUMNS) : base(manager)
        {
            _scroll = new ScrollContainer(manager)
            {
                Margin = new Border(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            _grid = new(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            _assets = assets;

            _scroll.Content = _grid;

            Controls.Add(_scroll);
            Rebuild(inventorySlots, columns);
        }

        /// <summary>
        ///     Gibt den aktuell selektierten Slot an.
        /// </summary>
        public InventorySlot HoveredSlot { get; private set; }

        public void Rebuild(List<InventorySlot> inventorySlots, int columns = COLUMNS)
        {
            _grid = new(ScreenManager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            _scroll.Content = _grid;

            for (var i = 0; i < columns; i++)
                _grid.Columns.Add(new ColumnDefinition { ResizeMode = ResizeMode.Parts, Width = 1 });

            var rows = (int)Math.Ceiling((float)inventorySlots.Count / columns);
            for (var i = 0; i < rows; i++)
                _grid.Rows.Add(new RowDefinition { ResizeMode = ResizeMode.Fixed, Height = 50 });

            var column = 0;
            var row = 0;
            foreach (var inventorySlot in inventorySlots)
            {
                if (inventorySlot.Definition is null)
                    continue;

                var texture = _assets.LoadTexture(inventorySlot.Definition.GetType(), inventorySlot.Definition.Icon);


                var image = new Image(ScreenManager)
                    { Texture = texture, Width = 42, Height = 42, VerticalAlignment = VerticalAlignment.Center };
                image.MouseEnter += (s, e) => { HoveredSlot = inventorySlot; };
                image.MouseLeave += (s, e) => { HoveredSlot = null; };
                image.StartDrag += (c, e) =>
                {
                    e.Handled = true;
                    e.Icon = texture;
                    e.Content = inventorySlot;
                    e.Sender = image;
                };
                var label = new Label(ScreenManager)
                {
                    Text = inventorySlot.Amount.ToString(), HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalTextAlignment = VerticalAlignment.Bottom, Background = new BorderBrush(Color.White)
                };
                _grid.AddControl(image, column, row);
                _grid.AddControl(label, column, row);

                column++;
                if (column >= columns)
                {
                    row++;
                    column = 0;
                }
            }
        }
    }
}