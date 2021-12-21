using System.Linq;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.UI.Languages;

namespace OctoAwesome.Client.Screens
{
    internal sealed class PauseScreen : Screen
    {
        public PauseScreen(ScreenComponent manager) : base(manager)
        {
            var assets = manager.Game.Assets;

            // IsOverlay = true;
            // Background = new BorderBrush(new Color(Color.Black, 0.5f));

            Background = new TextureBrush(assets.LoadTexture("background"), TextureBrushMode.Stretch);

            var stack = new StackPanel(manager);
            Controls.Add(stack);

            Button resumeButton = new TextButton(manager, OctoClient.Resume);
            resumeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            resumeButton.Margin = new(0, 0, 0, 10);
            resumeButton.LeftMouseClick += (s, e) => { manager.NavigateBack(); };
            stack.Controls.Add(resumeButton);

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

            Button mainMenuButton = new TextButton(manager, OctoClient.ToMainMenu);
            mainMenuButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainMenuButton.Margin = new(0, 0, 0, 10);
            mainMenuButton.LeftMouseClick += (s, e) =>
            {
                manager.Player.SetEntity(null);
                manager.Game.Simulation.ExitGame();

                foreach (var gameScreen in manager.History.OfType<GameScreen>()) gameScreen.Unload();

                manager.NavigateHome();
            };
            stack.Controls.Add(mainMenuButton);
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (Manager.CanGoBack && args.Key == Keys.Escape)
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyDown(args);
        }
    }
}