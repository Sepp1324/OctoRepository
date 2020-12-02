using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Screens;

namespace OctoAwesome.Client.Controls
{
    internal sealed class OptionsOptionControl : Panel
    {
        private readonly Label _rangeTitle;
        private readonly Textbox _mapPath;
        private readonly ISettings _settings;
        private readonly OptionsScreen _optionsScreen;

        public OptionsOptionControl(ScreenComponent manager, OptionsScreen optionsScreen) : base(manager)
        {
            _settings = manager.Game.Settings;
            _optionsScreen = optionsScreen;

            ////////////////////////////////////////////Settings Stack////////////////////////////////////////////
            var settingsStack = new StackPanel(manager)
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new Border(20, 20, 20, 20),
                Width = 650
            };
            Controls.Add(settingsStack);

            //////////////////////Viewrange//////////////////////
            var viewRange = _settings.Get<string>("Viewrange");

            _rangeTitle = new Label(manager)
            {
                Text = Languages.OctoClient.Viewrange + ": " + viewRange
            };
            settingsStack.Controls.Add(_rangeTitle);

            var viewRangeSLider = new Slider(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 20,
                Range = 9,
                Value = int.Parse(viewRange) - 1
            };
            viewRangeSLider.ValueChanged += (value) => SetViewrange(value + 1);
            settingsStack.Controls.Add(viewRangeSLider);


            //////////////////////Persistence//////////////////////
            var persistenceStack = new StackPanel(manager)
            {
                Orientation = Orientation.Horizontal,
                Margin = new Border(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(persistenceStack);

            var persistenceTitle = new Label(manager)
            {
                Text = Languages.OctoClient.DisablePersistence + ":"
            };
            persistenceStack.Controls.Add(persistenceTitle);

            var disablePersistence = new Checkbox(manager)
            {
                Checked = _settings.Get("DisablePersistence", false),
                HookBrush = new TextureBrush(manager.Game.Assets.LoadTexture(typeof(ScreenComponent), "iconCheck_brown"), TextureBrushMode.Stretch),
            };
            disablePersistence.CheckedChanged += (state) => SetPersistence(state);
            persistenceStack.Controls.Add(disablePersistence);

            //////////////////////Map Path//////////////////////
            var mapPathStack = new StackPanel(manager)
            {
                Orientation = Orientation.Vertical,
                Margin = new Border(0, 20, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            settingsStack.Controls.Add(mapPathStack);

            _mapPath = new Textbox(manager)
            {
                Text = _settings.Get<string>("ChunkRoot"),
                Enabled = false,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            mapPathStack.Controls.Add(_mapPath);

            Button changePath = new TextButton(manager, Languages.OctoClient.ChangePath);
            changePath.HorizontalAlignment = HorizontalAlignment.Center;
            changePath.Height = 40;
            changePath.LeftMouseClick += (s, e) => ChangePath();
            mapPathStack.Controls.Add(changePath);

            //////////////////////Fullscreen//////////////////////
            var fullscreenStack = new StackPanel(manager)
            {
                Orientation = Orientation.Horizontal,
                Margin = new Border(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(fullscreenStack);

            var fullscreenTitle = new Label(manager)
            {
                Text = Languages.OctoClient.EnableFullscreenOnStartup + ":"
            };
            fullscreenStack.Controls.Add(fullscreenTitle);

            var enableFullscreen = new Checkbox(manager)
            {
                Checked = _settings.Get<bool>("EnableFullscreen"),
                HookBrush = new TextureBrush(manager.Game.Assets.LoadTexture(typeof(ScreenComponent), "iconCheck_brown"), TextureBrushMode.Stretch),
            };
            enableFullscreen.CheckedChanged += (state) => SetFullscreen(state);
            fullscreenStack.Controls.Add(enableFullscreen);

            //////////////////////Auflösung//////////////////////
            var resolutionStack = new StackPanel(manager)
            {
                Orientation = Orientation.Horizontal,
                Margin = new Border(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(resolutionStack);

            var resolutionTitle = new Label(manager)
            {
                Text = Languages.OctoClient.Resolution + ":"
            };
            resolutionStack.Controls.Add(resolutionTitle);

            var resolutionWidthTextBox = new Textbox(manager)
            {
                Text = _settings.Get<string>("Width"),
                Width = 50,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            resolutionWidthTextBox.TextChanged += ResolutionWidthTextbox_TextChanged;
            resolutionStack.Controls.Add(resolutionWidthTextBox);

            var xLabel = new Label(manager)
            {
                Text = "x"
            };
            resolutionStack.Controls.Add(xLabel);

            var resolutionHeightTextBox = new Textbox(manager)
            {
                Text = _settings.Get<string>("Height"),
                Width = 50,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            resolutionHeightTextBox.TextChanged += ResolutionHeightTextbox_TextChanged;
            resolutionStack.Controls.Add(resolutionHeightTextBox);

            var pxLabel = new Label(manager)
            {
                Text = Languages.OctoClient.Pixels
            };
            resolutionStack.Controls.Add(pxLabel);
        }

        private void ResolutionWidthTextbox_TextChanged(Control sender, PropertyEventArgs<string> args)
        {
            _settings.Set("Width", args.NewValue);
            _optionsScreen.NeedRestart();
        }

        private void ResolutionHeightTextbox_TextChanged(Control sender, PropertyEventArgs<string> args)
        {
            _settings.Set("Height", args.NewValue);
            _optionsScreen.NeedRestart();
        }

        private void SetViewrange(int newRange)
        {
            _rangeTitle.Text = Languages.OctoClient.Viewrange + ": " + newRange;
            _settings.Set("Viewrange", newRange);
            _optionsScreen.NeedRestart();
        }

        private void ChangePath()
        {
            var folderBrowser = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = _settings.Get<string>("ChunkRoot")
            };

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = folderBrowser.SelectedPath;
                
                _settings.Set("ChunkRoot", path);
                _mapPath.Text = path;

                _optionsScreen.NeedRestart();
            }
        }

        private void SetPersistence(bool state)
        {
            _settings.Set("DisablePersistence", state);
            _optionsScreen.NeedRestart();
        }

        private void SetFullscreen(bool state)
        {
            _settings.Set("EnableFullscreen", state);
            _optionsScreen.NeedRestart();
        }
    }
}
