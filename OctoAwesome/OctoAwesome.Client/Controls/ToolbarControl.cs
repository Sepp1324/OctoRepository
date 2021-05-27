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
<<<<<<< HEAD
        private readonly Dictionary<string, Texture2D> _toolTextures;
        private readonly Button[] _buttons = new Button[ToolBarComponent.Toolcount];
        private readonly Image[] _images = new Image[ToolBarComponent.Toolcount];
        private readonly Brush _buttonBackground;
        private readonly Brush _activeBackground;

        private int _lastActiveIndex;

        private readonly Label _activeToolLabel;
        
        public PlayerComponent Player { get; set; }
=======
        private Dictionary<string, Texture2D> toolTextures;

        private Button[] buttons = new Button[ToolBarComponent.TOOLCOUNT];

        private Image[] images = new Image[ToolBarComponent.TOOLCOUNT];

        private Brush buttonBackgroud;

        private Brush activeBackground;

        public PlayerComponent Player { get; set; }

        public Label activeToolLabel;
>>>>>>> feature/performance

        public ToolbarControl(ScreenComponent screenManager) : base(screenManager)
        {
            Player = screenManager.Player;
            Player.Toolbar.OnChanged += SetTexture;
            
            _toolTextures = new Dictionary<string, Texture2D>();

            _buttonBackground = new BorderBrush(Color.Black);
            _activeBackground = new BorderBrush(Color.Red);

            foreach (var item in screenManager.Game.DefinitionManager.Definitions)
            {
<<<<<<< HEAD
                var texture = screenManager.Game.Assets.LoadTexture(item.GetType(), item.Icon);
                _toolTextures.Add(item.GetType().FullName, texture);
=======
                Texture2D texture = screenManager.Game.Assets.LoadTexture(item.GetType(), item.Icon);
                toolTextures.Add(item.GetType().FullName, texture);
>>>>>>> feature/performance
            }

            Grid grid = new Grid(screenManager)
            {
                Margin = new Border(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Controls.Add(grid);

            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });

<<<<<<< HEAD
            for (var i = 0; i < ToolBarComponent.Toolcount; i++)
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });
=======
            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            {
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });
            }
>>>>>>> feature/performance

            _activeToolLabel = new Label(screenManager) {VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Center, Background = new BorderBrush(Color.Black * 0.3f), TextColor = Color.White};
            grid.AddControl(_activeToolLabel, 0, 0, ToolBarComponent.Toolcount);

<<<<<<< HEAD
            for (var i = 0; i < ToolBarComponent.Toolcount; i++)
=======
            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
>>>>>>> feature/performance
            {
                _buttons[i] = new Button(screenManager)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = _buttonBackground,
                    HoveredBackground = null,
                    PressedBackground = null,
<<<<<<< HEAD
                    Content = _images[i] = new Image(screenManager) {Width = 42, Height = 42,},
=======
                };
                buttons[i].Content = images[i] = new Image(screenManager)
                {
                    Width = 42,
                    Height = 42,
>>>>>>> feature/performance
                };
                grid.AddControl(_buttons[i], i, 1);
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (!Visible || !Enabled) return;

            if (Player.CurrentEntity == null) return;

<<<<<<< HEAD
            if (Player.Toolbar.ActiveIndex != _lastActiveIndex)
=======
           // Aktualisierung des aktiven Buttons
            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
>>>>>>> feature/performance
            {
                _buttons[_lastActiveIndex].Background = _buttonBackground;
                _lastActiveIndex = Player.Toolbar.ActiveIndex;
            }
            
            _buttons[Player.Toolbar.ActiveIndex].Background = _activeBackground;

            SetTexture(Player.Toolbar.ActiveTool, Player.Toolbar.ActiveIndex);

            var newText = "";

            // Aktualisierung des ActiveTool Labels
<<<<<<< HEAD
            if (Player.Toolbar.ActiveTool != null)
            {
                newText = Player.Toolbar.ActiveTool.Definition.Name;

                if (Player.Toolbar.ActiveTool.Amount > 1)
                    newText += $" ({Player.Toolbar.ActiveTool.Amount})";
            }
=======
            activeToolLabel.Text = Player.Toolbar.ActiveTool != null ?
                string.Format("{0} ({1})", Player.Toolbar.ActiveTool.Definition.Name, Player.Toolbar.ActiveTool.Amount) :
                string.Empty;
>>>>>>> feature/performance

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
