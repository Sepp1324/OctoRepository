using System.Collections.Generic;
using System.Xml.Serialization;
using OctoAwesome.Client.Components;
using System;

namespace OctoAwesome.Client.Crew
{
    public class CrewMember
    {
        public enum Achievements
        {
            Patron,
            Streamer,
            HardwareDealer,
            Coder,
            Livegast,
            Tools,
            EmailJoker,
            Contributor,
            Musik,
            Grafik,
            WikiAdmin,
            DatenbankFreigeschaltet,
            Designer,
            Designerin,
            Tester,
            Testerin,
            MasterOfTheUniverse,
            MistressOfTheUniverse,
            Chef,
            Chefin
        };

        public string Username { get; set; }

        public string Alias { get; set; }

        public string Description { get; set; }

        public List<Achievements> AchievementList { get; set; }

        public string PictureFilename { get; set; }
        
        public List<Link> Links { get; set; }

        public CrewMember() { }

        internal static List<CrewMember> getCrew(ScreenComponent manager)
        {

            using (var stream = manager.Game.Assets.LoadStream(typeof(CrewMember), "crew", "xml"))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(List<CrewMember>));
                    return (List<CrewMember>)serializer.Deserialize(stream);
                }
                catch (Exception)
                { }

                return new List<CrewMember>();
            }
        }

        public override string ToString() => Username;

        public class Link
        {
            [XmlAttribute]
            public string Title { get; set; }

            [XmlAttribute]
            public string Url { get; set; }
        }
    }
}

    
