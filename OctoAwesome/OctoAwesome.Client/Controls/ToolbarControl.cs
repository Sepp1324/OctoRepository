using engenious.UI;
using OctoAwesome.Client.Components;
using System.Collections.Generic;
using engenious;
using engenious.Graphics;
using OctoAwesome.EntityComponents;
using engenious.UI.Controls;

namespace OctoAwesome.Client.Controls
{
    internal class ToolbarControl : Panel
    {
        private readonly Dictionary<string, Texture2D> _toolTextures;
        private readonly Button[] _buttons = new Button[ToolBarComponent.Toolcount];
        private readonly Image[] _images = new Image[ToolBarComponent.Toolcount];
        private readonly Brush _buttonBackground;
        private readonly Brush _activeBackground;

        private int _lastActiveIndex;

        private readonly Label _activeToolLabel;
        
        public PlayerComponent Player { get; set; }

        public ToolbarControl(ScreenComponent screenManager) : base(screenManager)
        {
            Player = screenManager.Player;
            _toolTextures = new Dictionary<string, Texture2D>();

            _buttonBackground = new BorderBrush(Color.Black);
            _activeBackground = new BorderBrush(Color.Red);

            foreach (var item in screenManager.Game.DefinitionManager.Definitions)
            {
                var texture = screenManager.Game.Assets.LoadTexture(item.GetType(), item.Icon);
                _toolTextures.Add(item.GetType().FullName, texture);
            }

            var grid = new Grid(screenManager)
            {
                Margin = new Border(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Controls.Add(grid);

            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });

            for (var i = 0; i < ToolBarComponent.Toolcount; i++)
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });

            _activeToolLabel = new Label(screenManager) {VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Center, Background = new BorderBrush(Color.Black * 0.3f), TextColor = Color.White};
            grid.AddControl(_activeToolLabel, 0, 0, ToolBarComponent.Toolcount);

            for (var i = 0; i < ToolBarComponent.Toolcount; i++)
            {
                _buttons[i] = new Button(screenManager)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = _buttonBackground,
                    HoveredBackground = null,
                    PressedBackground = null,
                    Content = _images[i] = new Image(screenManager) {Width = 42, Height = 42,},
                };
                grid.AddControl(_buttons[i], i, 1);
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (!Visible || !Enabled) return;

            if (Player.CurrentEntity == null) return;

            if (Player.Toolbar.ActiveIndex != _lastActiveIndex)
            {
                _buttons[_lastActiveIndex].Background = _buttonBackground;
                _lastActiveIndex = Player.Toolbar.ActiveIndex;
            }
            
            _buttons[Player.Toolbar.ActiveIndex].Background = _activeBackground;

            var definitionName = Player.Toolbar.ActiveTool.Definition.GetType().FullName;

            if (_toolTextures.TryGetValue(definitionName, out var texture))
                _images[Player.Toolbar.ActiveIndex].Texture = texture;

            // Aktualisierung des ActiveTool Labels
            _activeToolLabel.Text = Player.Toolbar.ActiveTool != null ? $"{Player.Toolbar.ActiveTool.Definition.Name} ({Player.Toolbar.ActiveTool.Amount})"
                : string.Empty;

            _activeToolLabel.Visible = _activeToolLabel.Text != string.Empty;

            base.OnUpdate(gameTime);
        }
    }
}
