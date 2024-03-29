﻿using System.Collections.Generic;
using engenious;
using engenious.Graphics;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Controls;
using OctoAwesome.UI.Languages;

namespace OctoAwesome.Client.Screens
{
    internal sealed class InventoryScreen : Screen
    {
        private readonly Image[] _images;

        private readonly InventoryControl _inventory;

        private readonly Label _massLabel;

        private readonly Label _nameLabel;

        private readonly PlayerComponent _player;
        private readonly Dictionary<string, Texture2D> _toolTextures = new();

        private readonly Label _volumeLabel;

        public InventoryScreen(ScreenComponent manager) : base(manager)
        {
            var assets = manager.Game.Assets;

            foreach (var item in manager.Game.DefinitionManager.Definitions)
            {
                var texture = manager.Game.Assets.LoadTexture(item.GetType(), item.Icon);
                _toolTextures.Add(item.GetType().FullName, texture);
            }

            _player = manager.Player;

            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.3f);

            Brush backgroundBrush = new BorderBrush(Color.Black);
            Brush hoverBrush = new BorderBrush(Color.Brown);

            var panelBackground = assets.LoadTexture("panel");

            var grid = new Grid(manager)
            {
                Width = 800,
                Height = 500
            };

            grid.Columns.Add(new() { ResizeMode = ResizeMode.Fixed, Width = 600 });
            grid.Columns.Add(new() { ResizeMode = ResizeMode.Fixed, Width = 200 });
            grid.Rows.Add(new() { ResizeMode = ResizeMode.Parts, Height = 1 });
            grid.Rows.Add(new() { ResizeMode = ResizeMode.Fixed, Height = 100 });

            Controls.Add(grid);

            _inventory = new InventoryControl(manager, assets, manager.Player.Inventory.Inventory)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20)
            };

            grid.AddControl(_inventory, 0, 0);

            var infoPanel = new StackPanel(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
                Margin = Border.All(10, 0, 0, 0)
            };

            _nameLabel = new(manager);
            infoPanel.Controls.Add(_nameLabel);
            _massLabel = new(manager);
            infoPanel.Controls.Add(_massLabel);
            _volumeLabel = new(manager);
            infoPanel.Controls.Add(_volumeLabel);
            grid.AddControl(infoPanel, 1, 0);

            var toolbar = new Grid(manager)
            {
                Margin = Border.All(0, 10, 0, 0),
                Height = 100,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30)
            };

            toolbar.Columns.Add(new() { ResizeMode = ResizeMode.Parts, Width = 1 });
            for (var i = 0; i < ToolBarComponent.TOOL_COUNT; i++)
                toolbar.Columns.Add(new() { ResizeMode = ResizeMode.Fixed, Width = 50 });
            toolbar.Columns.Add(new() { ResizeMode = ResizeMode.Parts, Width = 1 });
            toolbar.Rows.Add(new() { ResizeMode = ResizeMode.Parts, Height = 1 });

            _images = new Image[ToolBarComponent.TOOL_COUNT];
            for (var i = 0; i < ToolBarComponent.TOOL_COUNT; i++)
            {
                var image = _images[i] = new(manager)
                {
                    Width = 42,
                    Height = 42,
                    Background = backgroundBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Tag = i,
                    Padding = Border.All(2)
                };

                image.StartDrag += (c, e) =>
                {
                    var slot = _player.Toolbar.Tools[(int)image.Tag!];
                    if (slot != null)
                    {
                        e.Handled = true;
                        e.Icon = _toolTextures[slot.Definition.GetType().FullName!];
                        e.Content = slot;
                        e.Sender = toolbar;
                    }
                };

                image.DropEnter += (c, e) => { image.Background = hoverBrush; };
                image.DropLeave += (c, e) => { image.Background = backgroundBrush; };
                image.EndDrop += (c, e) =>
                {
                    e.Handled = true;

                    if (e.Sender is Grid) // && ShiftPressed
                    {
                        // Swap
                        var targetIndex = (int)image.Tag!;
                        var targetSlot = _player.Toolbar.Tools[targetIndex];

                        var sourceSlot = e.Content as InventorySlot;
                        var sourceIndex = _player.Toolbar.GetSlotIndex(sourceSlot);

                        _player.Toolbar.SetTool(sourceSlot, targetIndex);
                        _player.Toolbar.SetTool(targetSlot, sourceIndex);
                    }
                    else
                    {
                        // Inventory Drop
                        var slot = e.Content as InventorySlot;
                        _player.Toolbar.SetTool(slot, (int)image.Tag!);
                    }
                };

                toolbar.AddControl(image, i + 1, 0);
            }

            grid.AddControl(toolbar, 0, 1, 2);
            Title = OctoClient.Inventory;
        }

        protected override void OnEndDrop(DragEventArgs args)
        {
            base.OnEndDrop(args);

            if (args.Sender is not Grid)
                return;

            var slot = args.Content as InventorySlot;
            _player.Toolbar.RemoveSlot(slot);
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            // Tool neu zuweisen
            if ((int)args.Key >= (int)Keys.D0 && (int)args.Key <= (int)Keys.D9)
            {
                var offset = (int)args.Key - (int)Keys.D0;
                _player.Toolbar.SetTool(_inventory.HoveredSlot, offset);
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
            for (var i = 0; i < ToolBarComponent.TOOL_COUNT; i++)
                if (_player.Toolbar.Tools != null &&
                    _player.Toolbar.Tools.Length > i &&
                    _player.Toolbar.Tools[i] != null &&
                    _player.Toolbar.Tools[i].Item != null)
                    _images[i].Texture = _toolTextures[_player.Toolbar.Tools[i].Definition.GetType().FullName];
                else
                    _images[i].Texture = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            Manager.FreeMouse();
            base.OnNavigatedTo(args);
        }
    }
}