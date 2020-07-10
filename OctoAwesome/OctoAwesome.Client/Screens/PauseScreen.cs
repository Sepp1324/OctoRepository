using Microsoft.Xna.Framework;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System.Diagnostics;

namespace OctoAwesome.Client.Screens
{
    internal sealed class PauseScreen : Screen
    {
        public PauseScreen(ScreenComponent manager) : base(manager)
        {
            IsOverlay = true;
            Background = new BorderBrush(new Color(Color.Black, 0.5f));

            StackPanel stack = new StackPanel(manager);
            Controls.Add(stack);

            Button resumeButton = Button.TextButton(manager, "Resume");
            resumeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            resumeButton.Margin = new Border(0, 0, 0, 10);
            resumeButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            stack.Controls.Add(resumeButton);

            Button optionButton = Button.TextButton(manager, Languages.OctoClient.Options);
            optionButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            optionButton.Margin = new Border(0, 0, 0, 10);
            optionButton.MinWidth = 300;
            optionButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new OptionsScreen(manager));
            };
            stack.Controls.Add(optionButton);

            Button creditsButton = Button.TextButton(manager, Languages.OctoClient.CreditsCrew);
            creditsButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            creditsButton.Margin = new Border(0, 0, 0, 10);
            creditsButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new CreditsScreen(manager));
            };
            stack.Controls.Add(creditsButton);

            Button mainMenuButton = Button.TextButton(manager, "Main Menu");
            mainMenuButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainMenuButton.Margin = new Border(0, 0, 0, 10);
            mainMenuButton.LeftMouseClick += (s, e) => 
            {
                manager.Game.Simulation.ExitGame();
                manager.Exit(); 
            };
            stack.Controls.Add(mainMenuButton);
        }
    }
}
