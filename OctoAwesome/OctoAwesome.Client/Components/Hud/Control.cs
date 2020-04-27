using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctoAwesome.Client.Components.Hud
{
    internal abstract class Control : UiElement
    {
        public Index2 Position { get; set; }

        public Index2 Size { get; set; }

        public bool Enabled { get; set; }

        public bool Visible { get; set; }

        public Control(IScreenManager screenManager) : base(screenManager)
        {
            Enabled = true;
            Visible = true;
        }
    }
}
