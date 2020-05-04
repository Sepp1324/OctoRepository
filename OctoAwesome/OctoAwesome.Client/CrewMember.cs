using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Client.Components;

namespace OctoAwesome.Client
{
    class CrewMember
    {
        public enum Achievements
        {
            Entwickler,
            Designer,
            Tester
        };

        public string Username { get; set; }
        public string Alias { get; set; }

        public string Description { get; set; }

        public Dictionary<String, String> Urls { get; set; }

        public List<Achievements> AchievementList { get; set; }

        public Texture2D Picture { get; set; }

        public CrewMember(String username)
        {
            Username = username;
        }

        public CrewMember(String username, Texture2D picture)
        {
            Username = username;
            Picture = picture;
        }



        public static List<CrewMember> getCrew(ScreenComponent manager)
        {
            List<CrewMember> crew = new List<CrewMember>();

            CrewMember Sebastian = new CrewMember("Sebastian");
            Sebastian.Description = "Entwickler von OctoAwesome.";
            Sebastian.Alias = "Sepp";
            Sebastian.Urls = new Dictionary<string, string> { { "Test", "www.google.at" } };
            Sebastian.AchievementList = new List<Achievements> { Achievements.Entwickler };
            crew.Add(Sebastian);

            CrewMember Nicol = new CrewMember("Nicol");
            Nicol.Description = "Beste Designerin wo gibt <3";
            Nicol.Alias = "Nici";
            Nicol.Picture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/Crew/Nicol.jpg", manager.GraphicsDevice);

            Nicol.Urls = new Dictionary<string, string> { { "Blog", "www.google.at" } };
            Nicol.AchievementList = new List<Achievements> { Achievements.Designer };
            crew.Add(Nicol);

            CrewMember Christian = new CrewMember("Christian");
            Christian.Description = "Tester und zukünftiger Refaktorisierer von OctoAwesome.";
            Christian.Alias = "Chris";
            Christian.Picture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/Crew/Christian.jpg", manager.GraphicsDevice);
            Christian.Urls = new Dictionary<string, string> { { "Test", "www.google.at" } };
            Christian.AchievementList = new List<Achievements> { Achievements.Tester};
            crew.Add(Christian);

            //crew.Sort();
            return crew;

        }
    }
}

    
