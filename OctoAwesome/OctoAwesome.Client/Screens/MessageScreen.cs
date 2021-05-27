using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using System;

namespace OctoAwesome.Client.Screens
{
    internal sealed class MessageScreen : Screen
    {
<<<<<<< HEAD
        private readonly Panel _panel;
        private readonly AssetComponent _assets;
=======
        Panel panel;
        AssetComponent assets;
>>>>>>> feature/performance

        public MessageScreen(ScreenComponent manager, string title, string content, string buttonText = "OK", Action<Control, MouseEventArgs> buttonClick = null) : base(manager)
        {
            _assets = manager.Game.Assets;

            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.5f);
            Title = title;

            _panel = new Panel(manager)
            {
                Padding = Border.All(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Controls.Add(_panel);

<<<<<<< HEAD
            var spanel = new StackPanel(manager);
            _panel.Controls.Add(spanel);
=======
            StackPanel spanel = new StackPanel(manager);
            panel.Controls.Add(spanel);
>>>>>>> feature/performance

            Label headLine = new Label(manager)
            {
                Text = title,
                Font = Skin.Current.HeadlineFont,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            spanel.Controls.Add(headLine);

            Label contentLabel = new Label(manager)
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

<<<<<<< HEAD
            _panel.Background = NineTileBrush.FromSingleTexture(_assets.LoadTexture(typeof(ScreenComponent), "panel"), 30, 30);
=======
            panel.Background = NineTileBrush.FromSingleTexture(assets.LoadTexture(typeof(ScreenComponent), "panel"), 30, 30);
>>>>>>> feature/performance
        }
    }
}
