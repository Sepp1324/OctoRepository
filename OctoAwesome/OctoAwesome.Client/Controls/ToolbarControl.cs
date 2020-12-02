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
        private readonly Button[] _buttons = new Button[ToolBarComponent.TOOLCOUNT];
        private readonly Image[] _images = new Image[ToolBarComponent.TOOLCOUNT];
        private readonly Brush _buttonBackgroud;
        private readonly Brush _activeBackground;

        public PlayerComponent Player { get; set; }

        public Label activeToolLabel;

        public ToolbarControl(ScreenComponent screenManager) : base(screenManager)
        {
            Player = screenManager.Player;
            _toolTextures = new Dictionary<string, Texture2D>();

            _buttonBackgroud = new BorderBrush(Color.Black);
            _activeBackground = new BorderBrush(Color.Red);

            foreach (var item in screenManager.Game.DefinitionManager.GetDefinitions())
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

            for (var i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });

            activeToolLabel = new Label(screenManager)
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new BorderBrush(Color.Black * 0.3f),
                TextColor = Color.White
            };
            grid.AddControl(activeToolLabel, 0, 0, ToolBarComponent.TOOLCOUNT);

            for (var i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            {
                _buttons[i] = new Button(screenManager)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = _buttonBackgroud,
                    HoveredBackground = null,
                    PressedBackground = null,
                    Content = _images[i] = new Image(screenManager) { Width = 42, Height = 42, },
                };
                grid.AddControl(_buttons[i], i, 1);
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (!Visible || !Enabled)
                return;

            if (Player.CurrentEntity == null) return;

            for (var i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            {
                if (Player.Toolbar.Tools != null && Player.Toolbar.Tools.Length > i && Player.Toolbar.Tools[i] != null && Player.Toolbar.Tools[i].Definition != null)
                {
                    _images[i].Texture = _toolTextures[Player.Toolbar.Tools[i].Definition.GetType().FullName];

                    _buttons[i].Background = Player.Toolbar.ActiveTool == Player.Toolbar.Tools[i] ? _activeBackground : _buttonBackgroud;
                }
                else
                {
                    _images[i].Texture = null;
                    _buttons[i].Background = _buttonBackgroud;
                }
            }

            // Aktualisierung des ActiveTool Labels
            activeToolLabel.Text = Player.Toolbar.ActiveTool != null ? $"{Player.Toolbar.ActiveTool.Definition.Name} ({Player.Toolbar.ActiveTool.Amount})" : string.Empty;
            activeToolLabel.Visible = activeToolLabel.Text != string.Empty;

            base.OnUpdate(gameTime);
        }
    }
}
