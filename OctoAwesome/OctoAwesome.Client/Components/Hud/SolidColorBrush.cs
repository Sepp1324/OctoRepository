using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctoAwesome.Client.Components.Hud
{
    internal class SolidColorBrush : Brush
    {
        private Texture2D pix;

        public Color Color { get; set; }

        public SolidColorBrush(IScreenManager screenManager)
        {
            pix = screenManager.Pix;
        }

        public override void Draw(SpriteBatch batch, Rectangle rectangle)
        {
            batch.Begin();

            batch.Draw(pix, rectangle, Color);

            batch.End();
        }
    }
}
