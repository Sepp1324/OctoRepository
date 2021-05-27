using engenious.UI;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Controls;
using engenious.Graphics;
using engenious.UI.Controls;

namespace OctoAwesome.Client.Screens
{
    internal sealed class OptionsScreen : BaseScreen
    {
<<<<<<< HEAD
        private readonly AssetComponent _assets;
        private readonly Button _exitButton;
=======
        private AssetComponent assets;

        private Button exitButton;
>>>>>>> feature/performance

        public OptionsScreen(ScreenComponent manager) : base(manager)
        {
            _assets = manager.Game.Assets;

            Padding = new Border(0, 0, 0, 0);

            Title = Languages.OctoClient.Options;

<<<<<<< HEAD
            var panelBackground = _assets.LoadTexture(typeof(ScreenComponent), "panel");
=======
            Texture2D panelBackground = assets.LoadTexture(typeof(ScreenComponent), "panel");
>>>>>>> feature/performance

            SetDefaultBackground();

            TabControl tabs = new TabControl(manager)
            {
                Padding = new Border(20, 20, 20, 20),
                Width = 700,
                TabPageBackground = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
<<<<<<< HEAD
                TabBrush = NineTileBrush.FromSingleTexture(_assets.LoadTexture(typeof(ScreenComponent), "buttonLong_brown"), 15, 15),
                TabActiveBrush = NineTileBrush.FromSingleTexture(_assets.LoadTexture(typeof(ScreenComponent), "buttonLong_beige"), 15, 15),
=======
                TabBrush = NineTileBrush.FromSingleTexture(assets.LoadTexture(typeof(ScreenComponent), "buttonLong_brown"), 15, 15),
                TabActiveBrush = NineTileBrush.FromSingleTexture(assets.LoadTexture(typeof(ScreenComponent), "buttonLong_beige"), 15, 15),
>>>>>>> feature/performance
            };
            Controls.Add(tabs);

            #region OptionsPage

            TabPage optionsPage = new TabPage(manager, Languages.OctoClient.Options);
            tabs.Pages.Add(optionsPage);

            OptionsOptionControl optionsOptions = new OptionsOptionControl(manager, this)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            optionsPage.Controls.Add(optionsOptions);

            #endregion

            #region BindingsPage

<<<<<<< HEAD
            var bindingsPage = new TabPage(manager, Languages.OctoClient.KeyBindings) {Padding = Border.All(10)};
=======
            TabPage bindingsPage = new TabPage(manager, Languages.OctoClient.KeyBindings);
            bindingsPage.Padding = Border.All(10);
>>>>>>> feature/performance
            tabs.Pages.Add(bindingsPage);

            BindingsOptionControl bindingsOptions = new BindingsOptionControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            bindingsPage.Controls.Add(bindingsOptions);

            #endregion

            #region TexturePackPage

            TabPage resourcePackPage = new TabPage(manager, "Resource Packs");
            tabs.Pages.Add(resourcePackPage);

            ResourcePacksOptionControl resourcePacksOptions = new ResourcePacksOptionControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            resourcePackPage.Controls.Add(resourcePacksOptions);

            #endregion

            #region ExtensionPage

            TabPage extensionPage = new TabPage(manager, Languages.OctoClient.Extensions);
            tabs.Pages.Add(extensionPage);

            ExtensionsOptionControl extensionOptions = new ExtensionsOptionControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            extensionPage.Controls.Add(extensionOptions);

            #endregion

            ////////////////////////////////////////////Restart Button////////////////////////////////////////////
            _exitButton = new TextButton(manager, Languages.OctoClient.RestartGameToApplyChanges);
            _exitButton.VerticalAlignment = VerticalAlignment.Top;
            _exitButton.HorizontalAlignment = HorizontalAlignment.Right;
            _exitButton.Enabled = false;
            _exitButton.Visible = false;
            _exitButton.LeftMouseClick += (s, e) => Program.Restart();
            _exitButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(_exitButton);
        }

        public void NeedRestart()
        {
<<<<<<< HEAD
            _exitButton.Visible = true;
            _exitButton.Enabled = true;
=======
            exitButton.Visible = true;
            exitButton.Enabled = true;
>>>>>>> feature/performance
        }        
    }
}
