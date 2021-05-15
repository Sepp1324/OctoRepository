using System;
using System.Linq;
using engenious.UI;
using OctoAwesome.Client.Components;
using engenious.Graphics;
using System.Diagnostics;
using OctoAwesome.Client.Crew;
using engenious.UI.Controls;

namespace OctoAwesome.Client.Screens
{
    internal class CrewMemberScreen : BaseScreen
    {
        private readonly AssetComponent _assets;

        public CrewMemberScreen(ScreenComponent manager, CrewMember member) : base(manager)
        {
            _assets = manager.Game.Assets;

            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            Title = Languages.OctoClient.CreditsCrew + ": " + member.Username;

            var boldFont = manager.Content.Load<SpriteFont>("Fonts/BoldFont");

            Padding = new Border(0, 0, 0, 0);

            SetDefaultBackground();

            //The Panel
            var panelBackground = _assets.LoadTexture(typeof(ScreenComponent), "panel");
            var panel = new Panel(manager)
            {
                MaxWidth = 750,                
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = new Border(15, 15, 15, 15),
            };
            Controls.Add(panel);

            //The Main Stack - Split the Panel in half Horizontal
            var horizontalStack = new StackPanel(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Horizontal
            };
            panel.Controls.Add(horizontalStack);


            //The Profile Image
            var profileImage = new Image(manager)
            {
                Height = 200,
                Width = 200,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Border(0, 0, 10, 0)
            };
            profileImage.Texture = _assets.LoadTexture(typeof(CrewMember), member.PictureFilename ?? "base");
            horizontalStack.Controls.Add(profileImage);

            //The Text Stack
            var textStack = new StackPanel(manager) {VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Left, Width = 430};
            horizontalStack.Controls.Add(textStack);

            //The Username & Alias
            var usernameText = member.Username;
            
            if (member.Alias != member.Username)
                usernameText += " (" + member.Alias + ")";
           
            var username = new Label(manager)
            {
                Text = usernameText,
                Font = manager.Content.Load<SpriteFont>("Fonts/HeadlineFont"),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            textStack.Controls.Add(username);

            //Achievements
            var achievementString = string.Join(", ", member.AchievementList.Select(a => a.ToString()));

            var achievementStack = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Horizontal,
            };
            textStack.Controls.Add(achievementStack);

            var achievementsTitle = new Label(manager) { Text = Languages.OctoClient.Achievements + ": ", Font = boldFont, HorizontalAlignment = HorizontalAlignment.Left };
            achievementStack.Controls.Add(achievementsTitle);
            var achievements = new Label(manager) { Text = achievementString, HorizontalAlignment = HorizontalAlignment.Left };            
            achievementStack.Controls.Add(achievements);

            // Links
            var linkString = string.Join(", ", member.Links.Select(a => a.Title));

            var linkStack = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Horizontal,
            };
            textStack.Controls.Add(linkStack);

            var linkTitle = new Label(manager) { Text = Languages.OctoClient.Links + ": ", Font = boldFont, HorizontalAlignment = HorizontalAlignment.Left };
            linkStack.Controls.Add(linkTitle);

            foreach (var link in member.Links)
            {                
                if (CheckHttpUrl(link.Url))
                {
                    Button linkButton = new TextButton(manager, link.Title);
                    linkButton.LeftMouseClick += (s, e) => Process.Start(link.Url);
                    linkStack.Controls.Add(linkButton);
                }
            }

            var descriptionPanel = new Panel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            textStack.Controls.Add(descriptionPanel);

            var description = new Label(manager)
            {
                Text = member.Description,
                WordWrap = true,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Left,
                HorizontalTextAlignment = HorizontalAlignment.Left,
                VerticalTextAlignment = VerticalAlignment.Top,
            };
            description.InvalidateDimensions();
            descriptionPanel.Controls.Add(description);

            panel.Width = 700;
        }

        private bool CheckHttpUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out var tmp) && (tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps);
    }
}
