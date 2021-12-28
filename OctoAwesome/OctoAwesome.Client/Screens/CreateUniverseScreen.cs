using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.UI.Languages;

namespace OctoAwesome.Client.Screens
{
    internal class CreateUniverseScreen : BaseScreen
    {
        private readonly Button _createButton;
        private readonly ScreenComponent _manager;

        public CreateUniverseScreen(ScreenComponent manager) : base(manager)
        {
            _manager = manager;
            ISettings settings = manager.Game.Settings;

            Padding = new(0, 0, 0, 0);

            Title = OctoClient.CreateUniverse;

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

            grid.Columns.Add(new() { ResizeMode = ResizeMode.Auto });
            grid.Columns.Add(new() { Width = 1, ResizeMode = ResizeMode.Parts });

            var nameInput = GetTextbox();
            nameInput.TextChanged += (s, e) => { _createButton.Visible = !string.IsNullOrEmpty(e.NewValue); };
            AddLabeledControl(grid, $"{OctoClient.Name}: ", nameInput);

            var seedInput = GetTextbox();
            AddLabeledControl(grid, $"{OctoClient.Seed}: ", seedInput);

            _createButton = new TextButton(manager, OctoClient.Create);
            _createButton.HorizontalAlignment = HorizontalAlignment.Right;
            _createButton.VerticalAlignment = VerticalAlignment.Bottom;
            _createButton.Visible = false;
            _createButton.LeftMouseClick += (s, e) =>
            {
                if (string.IsNullOrEmpty(nameInput.Text))
                    return;

                manager.Player.SetEntity(null);

                var guid = _manager.Game.Simulation.NewGame(nameInput.Text, seedInput.Text);
                settings.Set("LastUniverse", guid.ToString());

                var player = manager.Game.Simulation.LoginPlayer("");
                manager.Game.Player.SetEntity(player);

                manager.NavigateToScreen(new LoadingScreen(manager));
            };
            panel.Controls.Add(_createButton);
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