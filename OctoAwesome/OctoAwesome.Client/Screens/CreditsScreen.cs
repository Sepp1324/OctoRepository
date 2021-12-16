using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Crew;
using OctoAwesome.UI.Languages;

namespace OctoAwesome.Client.Screens
{
    internal class CreditsScreen : BaseScreen
    {
        public CreditsScreen(ScreenComponent manager) : base(manager)
        {
            Padding = new Border(0, 0, 0, 0);

            Title = OctoClient.CreditsCrew;

            SetDefaultBackground();

            var crew = CrewMember.getCrew(manager);

            var crewScroll = new ScrollContainer(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Border(10, 10, 10, 10),
                CanFocus = false
            };

            var crewList = new StackPanel(manager)
            {
                MinWidth = 700,
                Padding = new Border(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Stretch,
                Orientation = Orientation.Vertical
            };
            crewScroll.Content = crewList;

            foreach (var member in crew)
            {
                Button memberButton = new TextButton(manager, member.Username);
                memberButton.HorizontalAlignment = HorizontalAlignment.Stretch;
                memberButton.Margin = new Border(5, 5, 5, 5);

                memberButton.LeftMouseClick += (s, e) =>
                {
                    manager.NavigateToScreen(new CrewMemberScreen(manager, member));
                };

                crewList.Controls.Add(memberButton);
            }


            Controls.Add(crewScroll);
        }
    }
}