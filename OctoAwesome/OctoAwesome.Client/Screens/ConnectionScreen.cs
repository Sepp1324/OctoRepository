﻿using System;
using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.UI.Languages;

namespace OctoAwesome.Client.Screens
{
    internal sealed class ConnectionScreen : BaseScreen
    {
        public ConnectionScreen(ScreenComponent manager) : base(manager)
        {
            var game = Manager.Game;
            Padding = new(0, 0, 0, 0);

            Title = OctoClient.CreateUniverse;

            SetDefaultBackground();

            var panel = new StackPanel(manager)
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

            var serverNameInput = new Textbox(manager)
            {
                Text = game.Settings.Get("server", "localhost"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black)
            };
            AddLabeledControl(grid, "Host:", serverNameInput);

            var playerNameInput = new Textbox(manager)
            {
                Text = game.Settings.Get("player", "USERNAME"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black)
            };
            AddLabeledControl(grid, "Username:", playerNameInput);

            var createButton = new TextButton(manager, OctoClient.Connect)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Visible = true
            };
            createButton.LeftMouseClick += (s, e) =>
            {
                game.Settings.Set("server", serverNameInput.Text);
                game.Settings.Set("player", playerNameInput.Text);

                ((ContainerResourceManager)game.ResourceManager).CreateManager(true);

                PlayMultiplayer(manager, playerNameInput.Text);
            };

            grid.Rows.Add(new() { ResizeMode = ResizeMode.Auto });
            grid.AddControl(createButton, 1, grid.Rows.Count - 1);
        }

        public new ScreenComponent Manager => (ScreenComponent)base.Manager;

        private void PlayMultiplayer(ScreenComponent manager, string playerName)
        {
            Manager.Player.SetEntity(null);

            Manager.Game.Simulation.LoadGame(Guid.Empty);
            //settings.Set("LastUniverse", levelList.SelectedItem.Id.ToString());

            var player = Manager.Game.Simulation.LoginPlayer(playerName);
            Manager.Game.Player.SetEntity(player);

            Manager.NavigateToScreen(new GameScreen(manager));
        }
    }
}