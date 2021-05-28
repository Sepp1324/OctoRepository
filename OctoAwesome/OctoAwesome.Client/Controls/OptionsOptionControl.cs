using System;
using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Languages;
using OctoAwesome.Client.Screens;
using Button = engenious.UI.Controls.Button;
using Control = engenious.UI.Control;
using HorizontalAlignment = engenious.UI.HorizontalAlignment;
using Label = engenious.UI.Controls.Label;
using Orientation = engenious.UI.Orientation;
using Panel = engenious.UI.Controls.Panel;

namespace OctoAwesome.Client.Controls
{
    internal sealed class OptionsOptionControl : Panel
    {
        private readonly OptionsScreen _optionsScreen;
        private readonly Label _rangeTitle;

        private readonly ISettings _settings;

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
            var viewrange = _settings.Get<string>("Viewrange");

            _rangeTitle = new Label(manager)
            {
                Text = OctoClient.Viewrange + ": " + viewrange
            };
            settingsStack.Controls.Add(_rangeTitle);

            var viewrangeSlider = new Slider(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 20,
                Range = 9,
                Value = int.Parse(viewrange) - 1
            };
            viewrangeSlider.ValueChanged += value => SetViewrange(value + 1);
            settingsStack.Controls.Add(viewrangeSlider);


            //////////////////////Persistence//////////////////////
            var persistenceStack = new StackPanel(manager)
            {
                Orientation = Orientation.Horizontal,
                Margin = new Border(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(persistenceStack);

            var persistenceTitle = new Label(manager)
            {
                Text = OctoClient.DisablePersistence + ":"
            };
            persistenceStack.Controls.Add(persistenceTitle);

            var disablePersistence = new Checkbox(manager)
            {
                Checked = _settings.Get("DisablePersistence", false),
                HookBrush = new TextureBrush(manager.Game.Assets.LoadTexture(typeof(ScreenComponent), "iconCheck_brown"), TextureBrushMode.Stretch)
            };
            disablePersistence.CheckedChanged += state => SetPersistence(state);
            persistenceStack.Controls.Add(disablePersistence);

            //////////////////////Map Path//////////////////////
            var mapPathStack = new StackPanel(manager)
            {
                Orientation = Orientation.Vertical,
                Margin = new Border(0, 20, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            settingsStack.Controls.Add(mapPathStack);

            var mapPath = new Textbox(manager)
            {
                Text = _settings.Get<string>("ChunkRoot"),
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
                Margin = new Border(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(fullscreenStack);

            var fullscreenTitle = new Label(manager)
            {
                Text = OctoClient.EnableFullscreenOnStartup + ":"
            };
            fullscreenStack.Controls.Add(fullscreenTitle);

            var enableFullscreen = new Checkbox(manager)
            {
                Checked = _settings.Get<bool>("EnableFullscreen"),
                HookBrush = new TextureBrush(manager.Game.Assets.LoadTexture(typeof(ScreenComponent), "iconCheck_brown"), TextureBrushMode.Stretch)
            };
            enableFullscreen.CheckedChanged += state => SetFullscreen(state);
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
                Text = OctoClient.Resolution + ":"
            };
            resolutionStack.Controls.Add(resolutionTitle);

            var resolutionWidthTextbox = new Textbox(manager)
            {
                Text = _settings.Get<string>("Width"),
                Width = 50,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            resolutionWidthTextbox.TextChanged += ResolutionWidthTextbox_TextChanged;
            resolutionStack.Controls.Add(resolutionWidthTextbox);

            var xLabel = new Label(manager)
            {
                Text = "x"
            };
            resolutionStack.Controls.Add(xLabel);

            var resolutionHeightTextbox = new Textbox(manager)
            {
                Text = _settings.Get<string>("Height"),
                Width = 50,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            resolutionHeightTextbox.TextChanged += ResolutionHeightTextbox_TextChanged;
            resolutionStack.Controls.Add(resolutionHeightTextbox);

            var pxLabel = new Label(manager)
            {
                Text = OctoClient.Pixels
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
            _rangeTitle.Text = OctoClient.Viewrange + ": " + newRange;

            _settings.Set("Viewrange", newRange);

            _optionsScreen.NeedRestart();
        }

        private void ChangePath() => throw new NotSupportedException();

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