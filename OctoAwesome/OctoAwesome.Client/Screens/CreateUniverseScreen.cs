using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using System;

namespace OctoAwesome.Client.Screens
{
    class CreateUniverseScreen : BaseScreen
    {
<<<<<<< HEAD
        private new readonly ScreenComponent _manager;
        private readonly Textbox _nameInput;
        private readonly Textbox _seedInput;
        readonly Button _createButton;
=======
        new readonly ScreenComponent Manager;
        private readonly Textbox nameInput;
        private readonly Textbox seedInput;
        readonly Button createButton;
>>>>>>> feature/performance

        private readonly ISettings _settings;

        public CreateUniverseScreen(ScreenComponent manager) : base(manager)
        {
            _manager = manager;
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

<<<<<<< HEAD
            _nameInput = GetTextbox();
            _nameInput.TextChanged += (s, e) =>
            {
                _createButton.Visible = !string.IsNullOrEmpty(e.NewValue);
            };
            AddLabeledControl(grid, $"{Languages.OctoClient.Name}: ", _nameInput);
=======
            nameInput = GetTextbox();
            nameInput.TextChanged += (s, e) =>
            {
                createButton.Visible = !string.IsNullOrEmpty(e.NewValue);
            };
            AddLabeledControl(grid, string.Format("{0}: ", Languages.OctoClient.Name), nameInput);
>>>>>>> feature/performance

            _seedInput = GetTextbox();
            AddLabeledControl(grid, $"{Languages.OctoClient.Seed}: ", _seedInput);

            _createButton = new TextButton(manager, Languages.OctoClient.Create) {HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Visible = false};
            _createButton.LeftMouseClick += (s, e) =>
            {
                if (string.IsNullOrEmpty(_nameInput.Text))
                    return;
                
                manager.Player.SetEntity(null);

<<<<<<< HEAD
                Guid guid = _manager.Game.Simulation.NewGame(_nameInput.Text, _seedInput.Text);
                _settings.Set("LastUniverse", guid.ToString());
=======
                Guid guid = Manager.Game.Simulation.NewGame(nameInput.Text, seedInput.Text);
                settings.Set("LastUniverse", guid.ToString());
>>>>>>> feature/performance

                Player player = manager.Game.Simulation.LoginPlayer("");
                manager.Game.Player.SetEntity(player);

                manager.NavigateToScreen(new LoadingScreen(manager));
            };
<<<<<<< HEAD
            panel.Controls.Add(_createButton);
=======
            panel.Controls.Add(createButton);
>>>>>>> feature/performance

        }

        private Textbox GetTextbox()
        {
            var t = new Textbox(_manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black)
            };
            return t;
        }
    }
}
