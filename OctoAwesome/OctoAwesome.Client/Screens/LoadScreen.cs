using System;
using System.Linq;
using engenious;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Languages;

namespace OctoAwesome.Client.Screens
{
    internal class LoadScreen : BaseScreen
    {
        private readonly Button createButton;
        private readonly Button deleteButton;
        private readonly Listbox<IUniverse> levelList;
        private readonly Grid mainStack;
        private new readonly ScreenComponent Manager;
        private readonly Button playButton;
        private readonly Label seedLabel;

        private readonly ISettings settings;

        public LoadScreen(ScreenComponent manager) : base(manager)
        {
            Manager = manager;
            settings = manager.Game.Settings;

            Padding = new Border(0, 0, 0, 0);

            Title = OctoClient.SelectUniverse;

            SetDefaultBackground();

            //Main Panel
            mainStack = new Grid(manager);
            mainStack.Columns.Add(new ColumnDefinition {ResizeMode = ResizeMode.Parts, Width = 3});
            mainStack.Columns.Add(new ColumnDefinition {ResizeMode = ResizeMode.Parts, Width = 1});
            mainStack.Rows.Add(new RowDefinition {ResizeMode = ResizeMode.Parts, Height = 1});
            mainStack.Margin = Border.All(50);
            mainStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainStack.VerticalAlignment = VerticalAlignment.Stretch;

            Controls.Add(mainStack);

            //Level Stack
            levelList = new Listbox<IUniverse>(manager)
            {
                Background = new BorderBrush(Color.White * 0.5f),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = Border.All(10),
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f)
            };
            levelList.TemplateGenerator += x =>
            {
                var li = new Label(manager)
                {
                    Text = string.Format("{0} ({1})", x.Name, x.Seed),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Padding = Border.All(10)
                };
                li.LeftMouseDoubleClick += (s, e) => Play();
                return li;
            };
            levelList.SelectedItemChanged += (s, e) =>
            {
                seedLabel.Text = "";
                if (levelList.SelectedItem != null)
                {
                    seedLabel.Text = "Seed: " + levelList.SelectedItem.Seed;
                    deleteButton.Enabled = true;
                }
                else
                {
                    deleteButton.Enabled = false;
                }
            };
            mainStack.AddControl(levelList, 0, 0);

            //Sidebar
            var sidebar = new Panel(manager);
            sidebar.Padding = Border.All(20);
            sidebar.VerticalAlignment = VerticalAlignment.Stretch;
            sidebar.HorizontalAlignment = HorizontalAlignment.Stretch;
            sidebar.Background = new BorderBrush(Color.White * 0.5f);
            sidebar.Margin = Border.All(10);
            mainStack.AddControl(sidebar, 1, 0);

            //Universe Info
            seedLabel = new Label(manager);
            seedLabel.Text = "";
            seedLabel.VerticalAlignment = VerticalAlignment.Top;
            seedLabel.HorizontalAlignment = HorizontalAlignment.Left;
            sidebar.Controls.Add(seedLabel);

            //Buttons
            var buttonStack = new StackPanel(manager);
            buttonStack.VerticalAlignment = VerticalAlignment.Bottom;
            buttonStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            sidebar.Controls.Add(buttonStack);

            //renameButton = getButton("Rename");
            //buttonStack.Controls.Add(renameButton);

            deleteButton = GetButton(OctoClient.Delete);
            deleteButton.Enabled = false;
            buttonStack.Controls.Add(deleteButton);
            deleteButton.LeftMouseClick += (s, e) =>
            {
                // Sicherstellen, dass universe nicht geladen ist
                if (Manager.Game.ResourceManager.CurrentUniverse != null &&
                    Manager.Game.ResourceManager.CurrentUniverse.Id == levelList.SelectedItem.Id)
                    return;

                Manager.Game.ResourceManager.DeleteUniverse(levelList.SelectedItem.Id);
                levelList.Items.Remove(levelList.SelectedItem);
                levelList.SelectedItem = null;
                levelList.InvalidateDimensions();
                settings.Set("LastUniverse", "");
            };

            createButton = GetButton(OctoClient.Create);
            createButton.LeftMouseClick += (s, e) => manager.NavigateToScreen(new CreateUniverseScreen(manager));
            buttonStack.Controls.Add(createButton);

            playButton = GetButton(OctoClient.Play);
            playButton.LeftMouseClick += (s, e) =>
            {
                if (levelList.SelectedItem == null)
                {
                    var msg = new MessageScreen(manager, OctoClient.Error, OctoClient.SelectUniverseFirst);
                    manager.NavigateToScreen(msg);

                    return;
                }

                Play();
            };
            buttonStack.Controls.Add(playButton);

            foreach (var universe in Manager.Game.ResourceManager.ListUniverses())
                levelList.Items.Add(universe);

            // Erstes Element auswählen, oder falls vorhanden das letzte gespielte Universum
            if (levelList.Items.Count >= 1)
                levelList.SelectedItem = levelList.Items[0];

            Guid lastUniverseId;
            if (Guid.TryParse(settings.Get<string>("LastUniverse"), out lastUniverseId))
            {
                var lastlevel = levelList.Items.FirstOrDefault(u => u.Id == lastUniverseId);
                if (lastlevel != null)
                    levelList.SelectedItem = lastlevel;
            }
        }


        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (args.Key == Keys.Enter)
            {
                if (levelList.SelectedItem == null)
                    return;

                Play();

                base.OnKeyDown(args);
            }
        }

        private void Play()
        {
            Manager.Player.SetEntity(null);

            Manager.Game.Simulation.LoadGame(levelList.SelectedItem.Id);
            settings.Set("LastUniverse", levelList.SelectedItem.Id.ToString());

            var player = Manager.Game.Simulation.LoginPlayer("");
            Manager.Game.Player.SetEntity(player);

            Manager.NavigateToScreen(new LoadingScreen(Manager));
        }
    }
}