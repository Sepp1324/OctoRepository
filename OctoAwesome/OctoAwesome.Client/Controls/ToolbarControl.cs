using System.Collections.Generic;
using engenious;
using engenious.Graphics;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

namespace OctoAwesome.Client.Controls
{
    internal class ToolbarControl : Panel
    {
        private readonly Brush _activeBackground;

        private readonly Label _activeToolLabel;

        private readonly Brush _buttonBackGround;

        private readonly Button[] _buttons = new Button[ToolBarComponent.TOOL_COUNT];

        private readonly Image[] _images = new Image[ToolBarComponent.TOOL_COUNT];

        private readonly Dictionary<string, Texture2D> _toolTextures;

        private int lastActiveIndex;

        public ToolbarControl(BaseScreenComponent screenManager, AssetComponent assets, PlayerComponent playerComponent, IDefinitionManager definitionManager) : base(screenManager)
        {
            Background = new SolidColorBrush(Color.Transparent);
            Player = playerComponent;
            Player.Toolbar.OnChanged += SetTexture;
            _toolTextures = new();

            _buttonBackGround = new BorderBrush(new(Color.Black, 0.5f));
            _activeBackground = new BorderBrush(new(Color.Black, 0.5f), LineType.Dotted, Color.Red, 3);

            foreach (var item in definitionManager.Definitions)
            {
                var texture = assets.LoadTexture(item.GetType(), item.Icon);
                _toolTextures.Add(item.GetType().FullName!, texture);
            }

            var grid = new Grid(screenManager)
            {
                Margin = new(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Controls.Add(grid);

            grid.Rows.Add(new() { ResizeMode = ResizeMode.Auto, Height = 1 });
            grid.Rows.Add(new() { ResizeMode = ResizeMode.Fixed, Height = 50 });

            for (var i = 0; i < ToolBarComponent.TOOL_COUNT; i++)
                grid.Columns.Add(new() { ResizeMode = ResizeMode.Fixed, Width = 50 });

            _activeToolLabel = new(screenManager)
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new BorderBrush(Color.Black * 0.3f),
                TextColor = Color.White
            };
            grid.AddControl(_activeToolLabel, 0, 0, ToolBarComponent.TOOL_COUNT);

            for (var i = 0; i < ToolBarComponent.TOOL_COUNT; i++)
            {
                _buttons[i] = new(screenManager)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = _buttonBackGround,
                    HoveredBackground = null!,
                    PressedBackground = null!,
                    Content = _images[i] = new(screenManager)
                    {
                        Width = 42,
                        Height = 42
                    }
                };
                grid.AddControl(_buttons[i], i, 1);
            }
        }

        public PlayerComponent Player { get; set; }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (!Visible || !Enabled)
                return;

            if (Player.CurrentEntity == null)
                return;

            if (Player.Toolbar.ActiveIndex != lastActiveIndex)
            {
                _buttons[lastActiveIndex].Background = _buttonBackGround;
                lastActiveIndex = Player.Toolbar.ActiveIndex;
            }

            _buttons[Player.Toolbar.ActiveIndex].Background = _activeBackground;
            SetTexture(Player.Toolbar.ActiveTool, Player.Toolbar.ActiveIndex);

            var newText = "";

            // Aktualisierung des ActiveTool Labels
            if (Player.Toolbar.ActiveTool != null)
            {
                newText = Player.Toolbar.ActiveTool.Definition.Name;

                if (Player.Toolbar.ActiveTool.Amount > 1)
                    newText += $" ({Player.Toolbar.ActiveTool.Amount})";
            }

            _activeToolLabel.Text = newText;

            _activeToolLabel.Visible = _activeToolLabel.Text != string.Empty;

            base.OnUpdate(gameTime);
        }

        private void SetTexture(InventorySlot inventorySlot, int index)
        {
            if (inventorySlot is null)
            {
                _images[index].Texture = null;
                return;
            }

            var definitionName = inventorySlot.Definition.GetType().FullName;

            _images[index].Texture = _toolTextures.TryGetValue(definitionName, out var texture) ? texture : null;
        }
    }
}