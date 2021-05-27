using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Controls;

namespace OctoAwesome.Client.Screens
{
    internal sealed class OptionsScreen : BaseScreen
    {
        private readonly AssetComponent assets;

        private readonly Button exitButton;

        public OptionsScreen(ScreenComponent manager) : base(manager)
        {
            assets = manager.Game.Assets;

            Padding = new Border(0, 0, 0, 0);

            Title = Languages.OctoClient.Options;

            var panelBackground = assets.LoadTexture(typeof(ScreenComponent), "panel");

            SetDefaultBackground();

            var tabs = new TabControl(manager)
            {
                Padding = new Border(20, 20, 20, 20),
                Width = 700,
                TabPageBackground = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                TabBrush = NineTileBrush.FromSingleTexture(assets.LoadTexture(typeof(ScreenComponent), "buttonLong_brown"), 15, 15),
                TabActiveBrush = NineTileBrush.FromSingleTexture(assets.LoadTexture(typeof(ScreenComponent), "buttonLong_beige"), 15, 15)
            };
            Controls.Add(tabs);

            #region OptionsPage

            var optionsPage = new TabPage(manager, Languages.OctoClient.Options);
            tabs.Pages.Add(optionsPage);

            var optionsOptions = new OptionsOptionControl(manager, this)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            optionsPage.Controls.Add(optionsOptions);

            #endregion

            #region BindingsPage

            var bindingsPage = new TabPage(manager, Languages.OctoClient.KeyBindings);
            bindingsPage.Padding = Border.All(10);
            tabs.Pages.Add(bindingsPage);

            var bindingsOptions = new BindingsOptionControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            bindingsPage.Controls.Add(bindingsOptions);

            #endregion

            #region TexturePackPage

            var resourcePackPage = new TabPage(manager, "Resource Packs");
            tabs.Pages.Add(resourcePackPage);

            var resourcePacksOptions = new ResourcePacksOptionControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            resourcePackPage.Controls.Add(resourcePacksOptions);

            #endregion

            #region ExtensionPage

            var extensionPage = new TabPage(manager, Languages.OctoClient.Extensions);
            tabs.Pages.Add(extensionPage);

            var extensionOptions = new ExtensionsOptionControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            extensionPage.Controls.Add(extensionOptions);

            #endregion

            ////////////////////////////////////////////Restart Button////////////////////////////////////////////
            exitButton = new TextButton(manager, Languages.OctoClient.RestartGameToApplyChanges);
            exitButton.VerticalAlignment = VerticalAlignment.Top;
            exitButton.HorizontalAlignment = HorizontalAlignment.Right;
            exitButton.Enabled = false;
            exitButton.Visible = false;
            exitButton.LeftMouseClick += (s, e) => Program.Restart();
            exitButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(exitButton);
        }

        public void NeedRestart()
        {
            exitButton.Visible = true;
            exitButton.Enabled = true;
        }
    }
}