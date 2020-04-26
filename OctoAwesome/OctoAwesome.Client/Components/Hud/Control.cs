using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctoAwesome.Client.Components.Hud
{
    internal abstract class Control
    {
        public Index2 Position { get; set; }

        public Index2 Size { get; set; }

        protected HudComponent Hud { get; private set; }

        public Control(HudComponent hud)
        {
            Hud = hud;
        }

        public abstract void LoadContent();

        public abstract void Draw(SpriteBatch batch);
    }
}
