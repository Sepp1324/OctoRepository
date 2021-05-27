using engenious.UI;
using OctoAwesome.Client.Components;
using engenious.Graphics;
using engenious.Input;
using System.Collections.Generic;
using OctoAwesome.Client.Controls;
using engenious;
using OctoAwesome.EntityComponents;
using engenious.UI.Controls;
using OctoAwesome.Definitions;

namespace OctoAwesome.Client.Screens
{
    internal sealed class InventoryScreen : Screen
    {
<<<<<<< HEAD
        private readonly Dictionary<string, Texture2D> _toolTextures = new Dictionary<string, Texture2D>();
        private readonly PlayerComponent _player;
        private readonly AssetComponent _assets;
        private readonly InventoryControl _inventory;
        private readonly Label _nameLabel;
        private readonly Label _massLabel;
        private readonly Label _volumeLabel;
        private readonly Image[] _images;
        private readonly Brush _backgroundBrush;
        private readonly Brush _hoverBrush;
        
=======
        private Dictionary<string, Texture2D> toolTextures = new Dictionary<string, Texture2D>();

        private PlayerComponent player;

        private AssetComponent assets;

        private InventoryControl inventory;

        private Label nameLabel;

        private Label massLabel;

        private Label volumeLabel;

        private Image[] images;

        private Brush backgroundBrush;

        private Brush hoverBrush;

>>>>>>> feature/performance
        public InventoryScreen(ScreenComponent manager) : base(manager)
        {
            _assets = manager.Game.Assets;

            foreach (var item in manager.Game.DefinitionManager.Definitions)
            {
<<<<<<< HEAD
                var texture = manager.Game.Assets.LoadTexture(item.GetType(), item.Icon);
                _toolTextures.Add(item.GetType().FullName, texture);
=======
                Texture2D texture = manager.Game.Assets.LoadTexture(item.GetType(), item.Icon);
                toolTextures.Add(item.GetType().FullName, texture);
>>>>>>> feature/performance
            }

            _player = manager.Player;

            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.3f);

            _backgroundBrush = new BorderBrush(Color.Black);
            _hoverBrush = new BorderBrush(Color.Brown);

<<<<<<< HEAD
            var panelBackground = _assets.LoadTexture(typeof(ScreenComponent), "panel");
=======
            Texture2D panelBackground = assets.LoadTexture(typeof(ScreenComponent), "panel");
>>>>>>> feature/performance

            Grid grid = new Grid(manager)
            {
                Width = 800,
                Height = 500,
            };

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 600 });
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 200 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 100 });

            Controls.Add(grid);

            _inventory = new InventoryControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            grid.AddControl(_inventory, 0, 0);

            StackPanel infoPanel = new StackPanel(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
                Margin = Border.All(10, 0, 0, 0),
            };

            _nameLabel = new Label(manager);
            infoPanel.Controls.Add(_nameLabel);
            _massLabel = new Label(manager);
            infoPanel.Controls.Add(_massLabel);
            _volumeLabel = new Label(manager);
            infoPanel.Controls.Add(_volumeLabel);
            grid.AddControl(infoPanel, 1, 0);

            Grid toolbar = new Grid(manager)
            {
                Margin = Border.All(0, 10, 0, 0),
                Height = 100,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
            };

            toolbar.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
<<<<<<< HEAD
            for (var i = 0; i < ToolBarComponent.Toolcount; i++)
=======
            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
>>>>>>> feature/performance
                toolbar.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });
            toolbar.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            toolbar.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });

<<<<<<< HEAD
            _images = new Image[ToolBarComponent.Toolcount];
            for (var i = 0; i < ToolBarComponent.Toolcount; i++)
            {
                var image = _images[i] = new Image(manager)
=======
            images = new Image[ToolBarComponent.TOOLCOUNT];
            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            {
                Image image = images[i] = new Image(manager)
>>>>>>> feature/performance
                {
                    Width = 42,
                    Height = 42,
                    Background = _backgroundBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Tag = i,
                    Padding = Border.All(2),
                };

                image.StartDrag += (e) =>
                {
<<<<<<< HEAD
                    var slot = _player.Toolbar.Tools[(int)image.Tag];
=======
                    InventorySlot slot = player.Toolbar.Tools[(int)image.Tag];
>>>>>>> feature/performance
                    if (slot != null)
                    {
                        e.Handled = true;
                        e.Icon = _toolTextures[slot.Definition.GetType().FullName];
                        e.Content = slot;
                        e.Sender = toolbar;
                    }
                };

                image.DropEnter += (e) => { image.Background = _hoverBrush; };
                image.DropLeave += (e) => { image.Background = _backgroundBrush; };
                image.EndDrop += (e) =>
                {
                    e.Handled = true;

                    if (e.Sender is Grid) // && ShiftPressed
                    {
                        // Swap
<<<<<<< HEAD
                        var targetIndex = (int)image.Tag;
                        var targetSlot = _player.Toolbar.Tools[targetIndex];

                        var sourceSlot = e.Content as InventorySlot;
                        var sourceIndex = _player.Toolbar.GetSlotIndex(sourceSlot);
=======
                        int targetIndex = (int)image.Tag;
                        InventorySlot targetSlot = player.Toolbar.Tools[targetIndex];

                        InventorySlot sourceSlot = e.Content as InventorySlot;
                        int sourceIndex = player.Toolbar.GetSlotIndex(sourceSlot);
>>>>>>> feature/performance

                        _player.Toolbar.SetTool(sourceSlot, targetIndex);
                        _player.Toolbar.SetTool(targetSlot, sourceIndex);
                    }
                    else
                    {
                        // Inventory Drop
                        InventorySlot slot = e.Content as InventorySlot;
<<<<<<< HEAD
                        _player.Toolbar.SetTool(slot, (int)image.Tag);
=======
                        player.Toolbar.SetTool(slot, (int)image.Tag);
>>>>>>> feature/performance
                    }
                };

                toolbar.AddControl(image, i + 1, 0);
            }

            grid.AddControl(toolbar, 0, 1, 2);
            Title = Languages.OctoClient.Inventory;
        }

        protected override void OnEndDrop(DragEventArgs args)
        {
            base.OnEndDrop(args);

            if (args.Sender is Grid)
            {
<<<<<<< HEAD
                var slot = args.Content as InventorySlot;
                _player.Toolbar.RemoveSlot(slot);
=======
                InventorySlot slot = args.Content as InventorySlot;
                player.Toolbar.RemoveSlot(slot);
>>>>>>> feature/performance
            }
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            // Tool neu zuweisen
            if ((int)args.Key >= (int)Keys.D0 && (int)args.Key <= (int)Keys.D9)
            {
<<<<<<< HEAD
                var offset = (int)args.Key - (int)Keys.D0;
                _player.Toolbar.SetTool(_inventory.HoveredSlot, offset);
=======
                int offset = (int)args.Key - (int)Keys.D0;
                player.Toolbar.SetTool(inventory.HoveredSlot, offset);
>>>>>>> feature/performance
                args.Handled = true;
            }

            if (Manager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.I))
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyDown(args);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            var name = _inventory.HoveredSlot?.Definition?.Name;

            if (_inventory.HoveredSlot?.Item is IItem item)
                name += " (" + item.Material.Name + ")";

            _nameLabel.Text = name ?? "";
            _massLabel.Text = _volumeLabel.Text = _inventory.HoveredSlot?.Amount.ToString() ?? "";

            // Aktualisierung des aktiven Buttons
<<<<<<< HEAD
            for (var i = 0; i < ToolBarComponent.Toolcount; i++)
=======
            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
>>>>>>> feature/performance
            {
                if (_player.Toolbar.Tools != null &&
                    _player.Toolbar.Tools.Length > i &&
                    _player.Toolbar.Tools[i] != null &&
                    _player.Toolbar.Tools[i].Item != null)
                {
                    _images[i].Texture = _toolTextures[_player.Toolbar.Tools[i].Definition.GetType().FullName];
                }
                else
                {
                    _images[i].Texture = null;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            Manager.FreeMouse();
            base.OnNavigatedTo(args);
        }
    }
}
