﻿using System;
using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.UI.Components;

namespace OctoAwesome.Client.Screens
{
    internal sealed class MessageScreen : Screen
    {
        public MessageScreen(BaseScreenComponent manager, AssetComponent assets, string title, string content, string buttonText = "OK", Action<Control, MouseEventArgs> buttonClick = null) : base(manager)
        {
            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.5f);
            Title = title;

            var panel = new Panel(manager)
            {
                Padding = Border.All(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Controls.Add(panel);

            var spanel = new StackPanel(manager);
            panel.Controls.Add(spanel);

            var headLine = new Label(manager)
            {
                Text = title,
                Font = Skin.Current.HeadlineFont,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            spanel.Controls.Add(headLine);

            var contentLabel = new Label(manager)
            {
                Text = content,
                Font = Skin.Current.TextFont,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            spanel.Controls.Add(contentLabel);

            Button closeButton = new TextButton(manager, buttonText);
            closeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            closeButton.LeftMouseClick += (s, e) =>
            {
                if (buttonClick != null)
                    buttonClick(s, e);
                else
                    manager.NavigateBack();
            };
            spanel.Controls.Add(closeButton);

            panel.Background = NineTileBrush.FromSingleTexture(assets.LoadTexture("panel"), 30, 30);
        }
    }
}