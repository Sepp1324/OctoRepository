using engenious;
using engenious.Graphics;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;

namespace OctoAwesome.Client.Controls
{
    internal sealed class InventoryControl : Panel
    {
        private const int COLUMNS = 8;

        public InventoryControl(ScreenComponent manager, int columns = COLUMNS) : base(manager)
        {
            var scroll = new ScrollContainer(manager)
            {
                Margin = new Border(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            Controls.Add(scroll);

            var grid = new Grid(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            for (var i = 0; i < columns; i++)
                grid.Columns.Add(new ColumnDefinition() {ResizeMode = ResizeMode.Parts, Width = 1});
            var rows = (int) System.Math.Ceiling((float) manager.Game.Player.Inventory.Inventory.Count / columns);
            for (var i = 0; i < rows; i++)
                grid.Rows.Add(new RowDefinition() {ResizeMode = ResizeMode.Fixed, Height = 50});

            var column = 0;
            var row = 0;
            foreach (var inventorySlot in manager.Game.Player.Inventory.Inventory)
            {
                Texture2D texture;
                if (inventorySlot.Definition is null)
                    continue;
                else
                    texture = manager.Game.Assets.LoadTexture(inventorySlot.Definition.GetType(), inventorySlot.Definition.Icon);


                var image = new Image(manager) {Texture = texture, Width = 42, Height = 42, VerticalAlignment = VerticalAlignment.Center};
                image.MouseEnter += (s, e) => { HoveredSlot = inventorySlot; };
                image.MouseLeave += (s, e) => { HoveredSlot = null; };
                image.StartDrag += (e) =>
                {
                    e.Handled = true;
                    e.Icon = texture;
                    e.Content = inventorySlot;
                    e.Sender = image;
                };
                var label = new Label(manager) {Text = inventorySlot.Amount.ToString(), HorizontalAlignment = HorizontalAlignment.Right, VerticalTextAlignment = VerticalAlignment.Bottom, Background = new BorderBrush(Color.White)};
                grid.AddControl(image, column, row);
                grid.AddControl(label, column, row);

                column++;
                if (column >= columns)
                {
                    row++;
                    column = 0;
                }
            }

            scroll.Content = grid;
        }

        /// <summary>
        /// Gibt den aktuell selektierten Slot an.
        /// </summary>
        public InventorySlot HoveredSlot { get; private set; }
    }
}