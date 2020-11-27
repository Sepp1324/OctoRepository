using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Linq;
using engenious;
using engenious.Input;

namespace OctoAwesome.Client.Screens
{
    internal class LoadScreen : BaseScreen
    {
        private new readonly ScreenComponent Manager;

        private readonly Button _deleteButton;
        private readonly Button _createButton;
        private readonly Button _playButton;
        private readonly Grid _mainStack;
        private readonly Listbox<IUniverse> _levelList;
        private readonly Label _seedLabel;
        private readonly ISettings _settings;

        public LoadScreen(ScreenComponent manager) : base(manager)
        {
            Manager = manager;
            _settings = manager.Game.Settings;

            Padding = new Border(0, 0, 0, 0);

            Title = Languages.OctoClient.SelectUniverse;

            SetDefaultBackground();

            //Main Panel
            _mainStack = new Grid(manager);
            _mainStack.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 3 });
            _mainStack.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            _mainStack.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            _mainStack.Margin = Border.All(50);
            _mainStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            _mainStack.VerticalAlignment = VerticalAlignment.Stretch;

            Controls.Add(_mainStack);

            //Level Stack
            _levelList = new Listbox<IUniverse>(manager)
            {
                Background = new BorderBrush(Color.White * 0.5f),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = Border.All(10),
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f)
            };

            _levelList.TemplateGenerator += (x) =>
            {
                var li = new Label(manager)
                {
                    Text = $"{x.Name} ({x.Seed})",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Padding = Border.All(10),
                };
                li.LeftMouseDoubleClick += (s, e) => Play();
                return li;
            };
            _levelList.SelectedItemChanged += (s, e) =>
            {
                _seedLabel.Text = "";
                if (_levelList.SelectedItem != null)
                    _seedLabel.Text = "Seed: " + _levelList.SelectedItem.Seed;
            };
            _mainStack.AddControl(_levelList, 0, 0);

            //Sidebar
            var sidebar = new Panel(manager)
            {
                Padding = Border.All(20),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new BorderBrush(Color.White * 0.5f),
                Margin = Border.All(10)
            };
            _mainStack.AddControl(sidebar, 1, 0);

            //Universe Info
            _seedLabel = new Label(manager)
            {
                Text = "", VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left
            };
            sidebar.Controls.Add(_seedLabel);

            //Buttons
            StackPanel buttonStack = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Stretch
            };
            sidebar.Controls.Add(buttonStack);

            //renameButton = getButton("Rename");
            //buttonStack.Controls.Add(renameButton);

            _deleteButton = GetButton(Languages.OctoClient.Delete);
            buttonStack.Controls.Add(_deleteButton);
            _deleteButton.LeftMouseClick += (s, e) =>
            {
                if (_levelList.SelectedItem == null)
                {
                    var msg = new MessageScreen(manager, Languages.OctoClient.Error, Languages.OctoClient.SelectUniverseFirst);
                    manager.NavigateToScreen(msg);

                    return;
                }

                // Sicherstellen, dass universe nicht geladen ist
                if (Manager.Game.ResourceManager.CurrentUniverse != null &&
                    Manager.Game.ResourceManager.CurrentUniverse.Id == _levelList.SelectedItem.Id)
                    return;

                Manager.Game.ResourceManager.DeleteUniverse(_levelList.SelectedItem.Id);
                _levelList.Items.Remove(_levelList.SelectedItem);
                _levelList.SelectedItem = null;
                _levelList.InvalidateDimensions();
                _settings.Set("LastUniverse", "");
            };

            _createButton = GetButton(Languages.OctoClient.Create);
            _createButton.LeftMouseClick += (s, e) => manager.NavigateToScreen(new CreateUniverseScreen(manager));
            buttonStack.Controls.Add(_createButton);

            _playButton = GetButton(Languages.OctoClient.Play);
            _playButton.LeftMouseClick += (s, e) =>
            {
                if (_levelList.SelectedItem == null)
                {
                    var msg = new MessageScreen(manager, Languages.OctoClient.Error, Languages.OctoClient.SelectUniverseFirst);
                    manager.NavigateToScreen(msg);

                    return;
                }

                Play();
            };
            buttonStack.Controls.Add(_playButton);

            foreach (var universe in Manager.Game.ResourceManager.ListUniverses())
                _levelList.Items.Add(universe);

            // Erstes Element auswählen, oder falls vorhanden das letzte gespielte Universum
            if (_levelList.Items.Count >= 1)
                _levelList.SelectedItem = _levelList.Items[0];

            Guid lastUniverseId;
            
            if (Guid.TryParse(_settings.Get<string>("LastUniverse"), out lastUniverseId))
            {
                var lastlevel = _levelList.Items.FirstOrDefault(u => u.Id == lastUniverseId);
                if (lastlevel != null)
                    _levelList.SelectedItem = lastlevel;

            }
        }

        private Button GetButton(string title)
        {
            var button = Button.TextButton(Manager, title);
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            return button;
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (args.Key == Keys.Enter)
            {
                if (_levelList.SelectedItem == null)
                    return;

                Play();

                base.OnKeyDown(args);
            }
        }

        private void Play()
        {
            Manager.Player.SetEntity(null);

            Manager.Game.Simulation.LoadGame(_levelList.SelectedItem.Id);
            _settings.Set("LastUniverse", _levelList.SelectedItem.Id.ToString());

            var player = Manager.Game.Simulation.LoginPlayer("");
            Manager.Game.Player.SetEntity(player);
            
            Manager.NavigateToScreen(new GameScreen(Manager));
        }
    }
}
