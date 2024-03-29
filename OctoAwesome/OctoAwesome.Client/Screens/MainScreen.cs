﻿using System.Diagnostics;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.UI.Languages;

namespace OctoAwesome.Client.Screens
{
    internal sealed class MainScreen : BaseScreen
    {
        public MainScreen(ScreenComponent manager) : base(manager)
        {
            var assets = manager.Game.Assets;

            Padding = new(0, 0, 0, 0);

            Background = new TextureBrush(assets.LoadTexture("background"), TextureBrushMode.Stretch);

            var stack = new StackPanel(manager);
            Controls.Add(stack);

            Button startButton = new TextButton(manager, OctoClient.Start);
            startButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            startButton.Margin = new(0, 0, 0, 10);
            startButton.LeftMouseClick += (s, e) =>
            {
                ((ContainerResourceManager)manager.Game.ResourceManager).CreateManager(false);
                manager.NavigateToScreen(new LoadScreen(manager));
            };
            stack.Controls.Add(startButton);

            Button multiplayerButton = new TextButton(manager, OctoClient.Multiplayer);
            multiplayerButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            multiplayerButton.Margin = new(0, 0, 0, 10);
            multiplayerButton.LeftMouseClick += (s, e) => { manager.NavigateToScreen(new ConnectionScreen(manager)); };
            stack.Controls.Add(multiplayerButton);

            Button optionButton = new TextButton(manager, OctoClient.Options);
            optionButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            optionButton.Margin = new(0, 0, 0, 10);
            optionButton.MinWidth = 300;
            optionButton.LeftMouseClick += (s, e) => { manager.NavigateToScreen(new OptionsScreen(manager)); };
            stack.Controls.Add(optionButton);

            Button creditsButton = new TextButton(manager, OctoClient.CreditsCrew);
            creditsButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            creditsButton.Margin = new(0, 0, 0, 10);
            creditsButton.LeftMouseClick += (s, e) => { manager.NavigateToScreen(new CreditsScreen(manager)); };
            stack.Controls.Add(creditsButton);

            Button webButton = new TextButton(manager, "Octoawesome.net");
            webButton.VerticalAlignment = VerticalAlignment.Bottom;
            webButton.HorizontalAlignment = HorizontalAlignment.Right;
            webButton.Margin = new(10, 10, 10, 10);
            webButton.LeftMouseClick += (s, e) => { Process.Start("http://octoawesome.net/"); };
            Controls.Add(webButton);

            Button exitButton = new TextButton(manager, OctoClient.Exit);
            exitButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            exitButton.Margin = new(0, 0, 0, 10);
            exitButton.LeftMouseClick += (s, e) => { manager.Exit(); };
            stack.Controls.Add(exitButton);
        }
    }
}