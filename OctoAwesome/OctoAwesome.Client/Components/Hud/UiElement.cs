using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctoAwesome.Client.Components.Hud
{
    internal abstract class UiElement
    {
        protected IScreenManager ScreenManager { get; private set; }

        public UiElement(IScreenManager screenManager)
        {
            ScreenManager = screenManager;
        }

        public virtual void LoadContent() { }

        public abstract void Draw(SpriteBatch batch, GameTime gameTime);
    }
}
