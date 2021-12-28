using System;
using System.Linq;
using engenious;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.UI.Languages;

namespace OctoAwesome.Client.Screens
{
    internal class LoadScreen : BaseScreen
    {
        private readonly Button _deleteButton;
        private readonly Listbox<IUniverse> _levelList;
        private readonly ScreenComponent _manager;
        private readonly Label _seedLabel;

        private readonly ISettings _settings;

        public LoadScreen(ScreenComponent manager) : base(manager)
        {
            _manager = manager;
            _settings = manager.Game.Settings;

            Padding = new(0, 0, 0, 0);

            Title = OctoClient.SelectUniverse;

            SetDefaultBackground();

            //Main Panel
            var mainStack = new Grid(manager);
            mainStack.Columns.Add(new() { ResizeMode = ResizeMode.Parts, Width = 3 });
            mainStack.Columns.Add(new() { ResizeMode = ResizeMode.Parts, Width = 1 });
            mainStack.Rows.Add(new() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainStack.Margin = Border.All(50);
            mainStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainStack.VerticalAlignment = VerticalAlignment.Stretch;

            Controls.Add(mainStack);

            //Level Stack
            _levelList = new(manager)
            {
                Background = new BorderBrush(Color.White * 0.5f),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = Border.All(10),
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f)
            };
            _levelList.TemplateGenerator += x =>
            {
                var li = new Label(manager)
                {
                    Text = $"{x?.Name} ({x?.Seed})",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Padding = Border.All(10)
                };
                li.LeftMouseDoubleClick += (s, e) => Play();
                return li;
            };
            _levelList.SelectedItemChanged += (s, e) =>
            {
                _seedLabel.Text = "";
                if (_levelList.SelectedItem != null)
                {
                    _seedLabel.Text = "Seed: " + _levelList.SelectedItem.Seed;
                    _deleteButton.Enabled = true;
                }
                else
                {
                    _deleteButton.Enabled = false;
                }
            };
            mainStack.AddControl(_levelList, 0, 0);

            //Sidebar
            var sidebar = new Panel(manager)
            {
                Padding = Border.All(20),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new BorderBrush(Color.White * 0.5f),
                Margin = Border.All(10)
            };
            mainStack.AddControl(sidebar, 1, 0);

            //Universe Info
            _seedLabel = new(manager)
            {
                Text = "",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sidebar.Controls.Add(_seedLabel);

            //Buttons
            var buttonStack = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            sidebar.Controls.Add(buttonStack);

            //renameButton = getButton("Rename");
            //buttonStack.Controls.Add(renameButton);

            _deleteButton = GetButton(OctoClient.Delete);
            _deleteButton.Enabled = false;
            buttonStack.Controls.Add(_deleteButton);
            _deleteButton.LeftMouseClick += (s, e) =>
            {
                // Sicherstellen, dass universe nicht geladen ist
                if (_manager.Game.ResourceManager.CurrentUniverse != null &&
                    _manager.Game.ResourceManager.CurrentUniverse.Id == _levelList?.SelectedItem?.Id)
                    return;

                _manager.Game.ResourceManager.DeleteUniverse(_levelList!.SelectedItem!.Id);
                _levelList.Items.Remove(_levelList.SelectedItem);
                _levelList.SelectedItem = null;
                _levelList.InvalidateDimensions();
                _settings.Set("LastUniverse", "");
            };

            var createButton = GetButton(OctoClient.Create);
            createButton.LeftMouseClick += (s, e) => manager.NavigateToScreen(new CreateUniverseScreen(manager));
            buttonStack.Controls.Add(createButton);

            var playButton = GetButton(OctoClient.Play);
            playButton.LeftMouseClick += (s, e) =>
            {
                if (_levelList.SelectedItem == null)
                {
                    var msg = new MessageScreen(manager, manager.Game.Assets, OctoClient.Error, OctoClient.SelectUniverseFirst);
                    manager.NavigateToScreen(msg);

                    return;
                }

                Play();
            };
            buttonStack.Controls.Add(playButton);

            foreach (var universe in _manager.Game.ResourceManager.ListUniverses())
                _levelList.Items.Add(universe);

            // Erstes Element auswählen, oder falls vorhanden das letzte gespielte Universum
            if (_levelList.Items.Count >= 1)
                _levelList.SelectedItem = _levelList.Items[0];

            if (Guid.TryParse(_settings.Get<string>("LastUniverse"), out var lastUniverseId))
            {
                var lastLevel = _levelList.Items.FirstOrDefault(u => u.Id == lastUniverseId);
                if (lastLevel != null)
                    _levelList.SelectedItem = lastLevel;
            }
        }


        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (args.Key != Keys.Enter) 
                return;

            if (_levelList.SelectedItem == null)
                return;

            Play();

            base.OnKeyDown(args);
        }

        private void Play()
        {
            _manager.Player.SetEntity(null);

            _manager.Game.Simulation.LoadGame(_levelList.SelectedItem!.Id);
            _settings.Set("LastUniverse", _levelList.SelectedItem.Id.ToString());

            var player = _manager.Game.Simulation.LoginPlayer("");
            _manager.Game.Player.SetEntity(player);

            _manager.NavigateToScreen(new LoadingScreen(_manager));
        }
    }
}