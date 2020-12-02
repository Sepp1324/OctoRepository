using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using System.Linq;

namespace OctoAwesome.Client.Screens
{
    internal sealed class PauseScreen : Screen
    {
        private readonly AssetComponent _assets;

        public PauseScreen(ScreenComponent manager) : base(manager)
        {
            _assets = manager.Game.Assets;

            // IsOverlay = true;
            // Background = new BorderBrush(new Color(Color.Black, 0.5f));

            Background = new TextureBrush(_assets.LoadTexture(typeof(ScreenComponent), "background"), TextureBrushMode.Stretch);

            var stack = new StackPanel(manager);
            Controls.Add(stack);

            Button resumeButton = new TextButton(manager, Languages.OctoClient.Resume)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Border(0, 0, 0, 10)
            };
            resumeButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            stack.Controls.Add(resumeButton);

            Button optionButton = new TextButton(manager, Languages.OctoClient.Options)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Border(0, 0, 0, 10),
                MinWidth = 300
            };
            optionButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new OptionsScreen(manager));
            };
            stack.Controls.Add(optionButton);

            Button creditsButton = new TextButton(manager, Languages.OctoClient.CreditsCrew)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Border(0, 0, 0, 10)
            };
            creditsButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new CreditsScreen(manager));
            };
            stack.Controls.Add(creditsButton);

            Button mainMenuButton = new TextButton(manager, Languages.OctoClient.ToMainMenu)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Border(0, 0, 0, 10)
            };
            mainMenuButton.LeftMouseClick += (s, e) =>
            {
                manager.Player.SetEntity(null);
                manager.Game.Simulation.ExitGame();

                foreach (var gameScreen in manager.History.OfType<GameScreen>())
                {
                    gameScreen.Unload();
                }

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
