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
            Tester,
            Supporter,
            Kritiker
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

            CrewMember Manu = new CrewMember("Manu");
            Manu.Description = "Tatkräftiger Unterstützer von OctoAwesome.";
            Manu.Picture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/Crew/Christian.jpg", manager.GraphicsDevice);
            Manu.Urls = new Dictionary<string, string> { { "Test", "www.google.at" } };
            Manu.AchievementList = new List<Achievements> { Achievements.Supporter };
            crew.Add(Manu);

            CrewMember Dave = new CrewMember("Dave");
            Dave.Description = "Hat immer ein kritisches Auge auf Manu";
            Dave.Alias = "Grafhugo";
            Dave.Picture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/Crew/Hugo.png", manager.GraphicsDevice);
            Dave.Urls = new Dictionary<string, string> { { "Test", "www.google.at" } };
            Dave.AchievementList = new List<Achievements> { Achievements.Kritiker };
            crew.Add(Dave);

            //crew.Sort();
            return crew;

        }
    }
}

    
