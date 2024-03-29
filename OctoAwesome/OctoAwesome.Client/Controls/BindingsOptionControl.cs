﻿using System.Linq;
using engenious;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Screens;
using OctoAwesome.UI.Components;
using OctoAwesome.UI.Languages;

namespace OctoAwesome.Client.Controls
{
    internal sealed class BindingsOptionControl : Panel
    {
        private readonly AssetComponent _assets;
        private readonly KeyMapper _keyMapper;
        private readonly ISettings _settings;

        public BindingsOptionControl(BaseScreenComponent manager, AssetComponent assets, KeyMapper keyMapper, ISettings settings) : base(manager)
        {
            _assets = assets;
            _settings = settings;
            _keyMapper = keyMapper;
            var bindingsScroll = new ScrollContainer(manager);
            Controls.Add(bindingsScroll);

            var bindingsStack = new StackPanel(manager)
            {
                Orientation = Orientation.Vertical,
                Padding = new(20, 20, 20, 20),
                Width = 650
            };
            bindingsScroll.Content = bindingsStack;

            //////////////////////////////KeyBindings////////////////////////////////////////////
            var bindings = keyMapper.GetBindings();
            foreach (var binding in bindings)
            {
                var bindingStack = new StackPanel(manager)
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 35
                };

                var lbl = new Label(manager)
                {
                    Text = binding.DisplayName,
                    Width = 480
                };

                var bindingKeyLabel = new Label(manager)
                {
                    Text = binding.Keys.First().ToString(),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Width = 90,
                    Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray),
                    Tag = new object[] { binding.Id, binding.Keys.First() }
                };
                bindingKeyLabel.LeftMouseClick += BindingKeyLabel_LeftMouseClick;

                bindingStack.Controls.Add(lbl);
                bindingStack.Controls.Add(bindingKeyLabel);
                bindingsStack.Controls.Add(bindingStack);
            }
        }

        private void BindingKeyLabel_LeftMouseClick(Control sender, MouseEventArgs args)
        {
            var data = (object[])sender.Tag;

            if (data is null)
                return;

            var id = (string)data[0];
            var oldKey = (Keys)data[1];

            var lbl = (Label)sender;

            var screen = new MessageScreen(ScreenManager, _assets, OctoClient.PressKey, "", OctoClient.Cancel);
            screen.KeyDown += (s, a) =>
            {
                _keyMapper.RemoveKey(id, oldKey);
                _keyMapper.AddKey(id, a.Key);
                data[1] = a.Key;
                _settings.Set("KeyMapper-" + id, a.Key.ToString());
                lbl.Text = a.Key.ToString();
                ScreenManager.NavigateBack();
            };
            ScreenManager.NavigateToScreen(screen);
        }
    }
}