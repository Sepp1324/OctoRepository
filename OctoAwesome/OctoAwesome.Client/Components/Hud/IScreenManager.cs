using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace OctoAwesome.Client.Components.Hud
{
    public interface IScreenManager
    {
        Texture2D Pix { get; }
    
        SpriteFont NormalText { get; }

        Index2 ScreenSize { get; }

        ContentManager Content { get; }

        GraphicsDevice GraphicsDevice { get; }

        void Close();
    }
}
