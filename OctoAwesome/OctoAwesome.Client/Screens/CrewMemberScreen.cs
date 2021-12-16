using System;
using System.Diagnostics;
using System.Linq;
using engenious.Graphics;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Crew;
using OctoAwesome.UI.Components;
using OctoAwesome.UI.Languages;

namespace OctoAwesome.Client.Screens
{
    internal class CrewMemberScreen : BaseScreen
    {
        private readonly AssetComponent assets;

        public CrewMemberScreen(ScreenComponent manager, CrewMember member) : base(manager)
        {
            assets = manager.Game.Assets;

            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            Title = OctoClient.CreditsCrew + ": " + member.Username;

            var boldFont = manager.Content.Load<SpriteFont>("Fonts/BoldFont");

            Padding = new Border(0, 0, 0, 0);

            SetDefaultBackground();

            //The Panel
            var panelBackground = assets.LoadTexture("panel");
            var panel = new Panel(manager)
            {
                MaxWidth = 750,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = new Border(15, 15, 15, 15)
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
            if (member.PictureFilename == null)
                profileImage.Texture = assets.LoadTexture(typeof(CrewMember), "base");
            else profileImage.Texture = assets.LoadTexture(typeof(CrewMember), member.PictureFilename);
            horizontalStack.Controls.Add(profileImage);

            //The Text Stack
            var textStack = new StackPanel(manager);
            textStack.VerticalAlignment = VerticalAlignment.Stretch;
            textStack.HorizontalAlignment = HorizontalAlignment.Left;
            textStack.Width = 430;
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
                Orientation = Orientation.Horizontal
            };
            textStack.Controls.Add(achievementStack);

            var achievementsTitle = new Label(manager)
            {
                Text = OctoClient.Achievements + ": ", Font = boldFont, HorizontalAlignment = HorizontalAlignment.Left
            };
            achievementStack.Controls.Add(achievementsTitle);
            var achievements = new Label(manager)
                { Text = achievementString, HorizontalAlignment = HorizontalAlignment.Left };
            achievementStack.Controls.Add(achievements);

            // Links
            var linkString = string.Join(", ", member.Links.Select(a => a.Title));

            var linkStack = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Horizontal
            };
            textStack.Controls.Add(linkStack);

            var linkTitle = new Label(manager)
                { Text = OctoClient.Links + ": ", Font = boldFont, HorizontalAlignment = HorizontalAlignment.Left };
            linkStack.Controls.Add(linkTitle);

            foreach (var link in member.Links)
                if (CheckHttpUrl(link.Url))
                {
                    Button linkButton = new TextButton(manager, link.Title);
                    linkButton.LeftMouseClick += (s, e) => Process.Start(link.Url);
                    linkStack.Controls.Add(linkButton);
                }

            var descriptionPanel = new Panel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            textStack.Controls.Add(descriptionPanel);

            var description = new Label(manager)
            {
                Text = member.Description,
                WordWrap = true,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Left,
                HorizontalTextAlignment = HorizontalAlignment.Left,
                VerticalTextAlignment = VerticalAlignment.Top
            };
            description.InvalidateDimensions();
            descriptionPanel.Controls.Add(description);

            panel.Width = 700;
        }

        private bool CheckHttpUrl(string url)
        {
            Uri tmp;
            return Uri.TryCreate(url, UriKind.Absolute, out tmp) &&
                   (tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps);
        }
    }
}