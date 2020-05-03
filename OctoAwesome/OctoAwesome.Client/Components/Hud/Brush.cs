using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctoAwesome.Client.Components.Hud
{
    internal abstract class Brush
    {
        public abstract void Draw(SpriteBatch batch, Rectangle rectangle);
    }
}
