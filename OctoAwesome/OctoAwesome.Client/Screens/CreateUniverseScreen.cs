using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using System;

namespace OctoAwesome.Client.Screens
{
    class CreateUniverseScreen : BaseScreen
    {
        new readonly ScreenComponent Manager;
        private readonly Textbox _nameInput;
        private readonly Textbox _seedInput;
        readonly Button _createButton;

        private readonly ISettings _settings;

        public CreateUniverseScreen(ScreenComponent manager) : base(manager)
        {
            Manager = manager;
            _settings = manager.Game.Settings;

            Padding = new Border(0, 0, 0, 0);

            Title = Languages.OctoClient.CreateUniverse;

            SetDefaultBackground();

            var panel = new Panel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = Border.All(50),
                Background = new BorderBrush(Color.White * 0.5f),
                Padding = Border.All(10)
            };
            Controls.Add(panel);

            var grid = new Grid(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            panel.Controls.Add(grid);

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Auto });
            grid.Columns.Add(new ColumnDefinition() { Width = 1, ResizeMode = ResizeMode.Parts });

            _nameInput = GetTextbox();
            _nameInput.TextChanged += (s, e) =>
            {
                _createButton.Visible = !string.IsNullOrEmpty(e.NewValue);
            };
            AddLabeledControl(grid, $"{Languages.OctoClient.Name}: ", _nameInput);

            _seedInput = GetTextbox();
            AddLabeledControl(grid, $"{Languages.OctoClient.Seed}: ", _seedInput);

            _createButton = new TextButton(manager, Languages.OctoClient.Create);
            _createButton.HorizontalAlignment = HorizontalAlignment.Right;
            _createButton.VerticalAlignment = VerticalAlignment.Bottom;
            _createButton.Visible = false;
            _createButton.LeftMouseClick += (s, e) =>
            {
                if (string.IsNullOrEmpty(_nameInput.Text))
                    return;
                
                manager.Player.SetEntity(null);

                var guid = Manager.Game.Simulation.NewGame(_nameInput.Text, _seedInput.Text);
                _settings.Set("LastUniverse", guid.ToString());

                var player = manager.Game.Simulation.LoginPlayer("");
                manager.Game.Player.SetEntity(player);

                manager.NavigateToScreen(new LoadingScreen(manager));
            };
            panel.Controls.Add(_createButton);
        }

        private Textbox GetTextbox()
        {
            var t = new Textbox(Manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black)
            };
            return t;
        }
    }
}
