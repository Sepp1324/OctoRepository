using System;
using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Screens;
using OctoAwesome.UI.Components;
using OctoAwesome.UI.Languages;

namespace OctoAwesome.Client.Controls
{
    internal sealed class OptionsOptionControl : Panel
    {
        private readonly OptionsScreen _optionsScreen;
        private readonly Label _rangeTitle;

        private readonly ISettings _settings;

        public OptionsOptionControl(BaseScreenComponent manager, OptionsScreen optionsScreen, ISettings settings, AssetComponent assets) : base(manager)
        {
            _settings = settings;
            _optionsScreen = optionsScreen;

            ////////////////////////////////////////////Settings Stack////////////////////////////////////////////
            var settingsStack = new StackPanel(manager)
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new(20, 20, 20, 20),
                Width = 650
            };
            Controls.Add(settingsStack);

            //////////////////////Viewrange//////////////////////
            var viewRange = settings.Get<string>("Viewrange");

            _rangeTitle = new(manager)
            {
                Text = OctoClient.Viewrange + ": " + viewRange
            };
            settingsStack.Controls.Add(_rangeTitle);

            var viewRangeSLider = new Slider(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 20,
                Range = 9,
                Value = int.Parse(viewRange) - 1
            };
            viewRangeSLider.ValueChanged += value => SetViewRange(value + 1);
            settingsStack.Controls.Add(viewRangeSLider);


            //////////////////////Persistence//////////////////////
            var persistenceStack = new StackPanel(manager)
            {
                Orientation = Orientation.Horizontal,
                Margin = new(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(persistenceStack);

            var persistenceTitle = new Label(manager)
            {
                Text = OctoClient.DisablePersistence + ":"
            };
            persistenceStack.Controls.Add(persistenceTitle);

            var disablePersistence = new Checkbox(manager)
            {
                Checked = settings.Get("DisablePersistence", false),
                HookBrush = new TextureBrush(assets.LoadTexture("iconCheck_brown"), TextureBrushMode.Stretch)
            };
            disablePersistence.CheckedChanged += SetPersistence;
            persistenceStack.Controls.Add(disablePersistence);

            //////////////////////Map Path//////////////////////
            var mapPathStack = new StackPanel(manager)
            {
                Orientation = Orientation.Vertical,
                Margin = new(0, 20, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            settingsStack.Controls.Add(mapPathStack);

            var mapPath = new Textbox(manager)
            {
                Text = settings.Get<string>("ChunkRoot"),
                Enabled = false,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            mapPathStack.Controls.Add(mapPath);

            Button changePath = new TextButton(manager, OctoClient.ChangePath);
            changePath.HorizontalAlignment = HorizontalAlignment.Center;
            changePath.Height = 40;
            changePath.LeftMouseClick += (s, e) => ChangePath();
            mapPathStack.Controls.Add(changePath);

            //////////////////////Fullscreen//////////////////////
            var fullscreenStack = new StackPanel(manager)
            {
                Orientation = Orientation.Horizontal,
                Margin = new(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(fullscreenStack);

            var fullscreenTitle = new Label(manager)
            {
                Text = OctoClient.EnableFullscreenOnStartup + ":"
            };
            fullscreenStack.Controls.Add(fullscreenTitle);

            var enableFullscreen = new Checkbox(manager)
            {
                Checked = settings.Get<bool>("EnableFullscreen"),
                HookBrush = new TextureBrush(assets.LoadTexture("iconCheck_brown"), TextureBrushMode.Stretch)
            };
            enableFullscreen.CheckedChanged += SetFullscreen;
            fullscreenStack.Controls.Add(enableFullscreen);

            //////////////////////Auflösung//////////////////////
            var resolutionStack = new StackPanel(manager)
            {
                Orientation = Orientation.Horizontal,
                Margin = new(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(resolutionStack);

            var resolutionTitle = new Label(manager)
            {
                Text = OctoClient.Resolution + ":"
            };
            resolutionStack.Controls.Add(resolutionTitle);

            var resolutionWidthTextBox = new Textbox(manager)
            {
                Text = settings.Get<string>("Width"),
                Width = 50,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            resolutionWidthTextBox.TextChanged += ResolutionWidthTextBox_TextChanged;
            resolutionStack.Controls.Add(resolutionWidthTextBox);

            var xLabel = new Label(manager)
            {
                Text = "x"
            };
            resolutionStack.Controls.Add(xLabel);

            var resolutionHeightTextBox = new Textbox(manager)
            {
                Text = settings.Get<string>("Height"),
                Width = 50,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            resolutionHeightTextBox.TextChanged += ResolutionHeightTextBox_TextChanged;
            resolutionStack.Controls.Add(resolutionHeightTextBox);

            var pxLabel = new Label(manager)
            {
                Text = OctoClient.Pixels
            };
            resolutionStack.Controls.Add(pxLabel);
        }

        private void ResolutionWidthTextBox_TextChanged(Control sender, PropertyEventArgs<string> args)
        {
            _settings.Set("Width", args.NewValue);

            _optionsScreen.NeedRestart();
        }

        private void ResolutionHeightTextBox_TextChanged(Control sender, PropertyEventArgs<string> args)
        {
            _settings.Set("Height", args.NewValue);

            _optionsScreen.NeedRestart();
        }

        private void SetViewRange(int newRange)
        {
            _rangeTitle.Text = OctoClient.Viewrange + ": " + newRange;

            _settings.Set("Viewrange", newRange);

            _optionsScreen.NeedRestart();
        }

        private void ChangePath()
        {
            throw new NotSupportedException();
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