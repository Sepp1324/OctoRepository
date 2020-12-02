using engenious.UI;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Controls;
using engenious.Graphics;
using engenious.UI.Controls;

namespace OctoAwesome.Client.Screens
{
    internal sealed class OptionsScreen : BaseScreen
    {
        private readonly AssetComponent _assets;
        private readonly Button _exitButton;

        public OptionsScreen(ScreenComponent manager) : base(manager)
        {
            _assets = manager.Game.Assets;

            Padding = new Border(0, 0, 0, 0);

            Title = Languages.OctoClient.Options;

            var panelBackground = _assets.LoadTexture(typeof(ScreenComponent), "panel");

            SetDefaultBackground();

            var tabs = new TabControl(manager)
            {
                Padding = new Border(20, 20, 20, 20),
                Width = 700,
                TabPageBackground = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                TabBrush = NineTileBrush.FromSingleTexture(_assets.LoadTexture(typeof(ScreenComponent), "buttonLong_brown"), 15, 15),
                TabActiveBrush = NineTileBrush.FromSingleTexture(_assets.LoadTexture(typeof(ScreenComponent), "buttonLong_beige"), 15, 15),
            };
            Controls.Add(tabs);

            #region OptionsPage

            var optionsPage = new TabPage(manager, Languages.OctoClient.Options);
            tabs.Pages.Add(optionsPage);

            var optionsOptions = new OptionsOptionControl(manager, this)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            optionsPage.Controls.Add(optionsOptions);

            #endregion

            #region BindingsPage

            var bindingsPage = new TabPage(manager, Languages.OctoClient.KeyBindings);
            tabs.Pages.Add(bindingsPage);

            var bindingsOptions = new BindingsOptionControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            bindingsPage.Controls.Add(bindingsOptions);

            #endregion

            #region TexturePackPage

            var resourcePackPage = new TabPage(manager, "Resource Packs");
            tabs.Pages.Add(resourcePackPage);

            var resourcePacksOptions = new ResourcePacksOptionControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            resourcePackPage.Controls.Add(resourcePacksOptions);

            #endregion

            #region ExtensionPage

            var extensionPage = new TabPage(manager, Languages.OctoClient.Extensions);
            tabs.Pages.Add(extensionPage);

            var extensionOptions = new ExtensionsOptionControl(manager)
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
            _exitButton.Visible = true;
            _exitButton.Enabled = true;
        }        
    }
}
