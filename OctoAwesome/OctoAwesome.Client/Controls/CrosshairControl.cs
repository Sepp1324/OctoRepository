using engenious.UI;
using OctoAwesome.Client.Components;
using engenious;
using engenious.Graphics;

namespace OctoAwesome.Client.Controls
{
    class CrosshairControl : Control
    {
<<<<<<< HEAD
        public Texture2D TEXTURE;
        public float TRANSPARENCY;
        public Color COLOR;

        readonly AssetComponent _assets;
=======
        public Texture2D Texture;
        public float Transparency;
        public Color Color;

        AssetComponent assets;
>>>>>>> feature/performance

        public CrosshairControl(ScreenComponent manager) : base(manager)
        {
            _assets = manager.Game.Assets;

            TRANSPARENCY = 0.5f;
            COLOR = Color.White;

            TEXTURE = _assets.LoadTexture(GetType(), "octocross");
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (!_assets.Ready)
                return;

            batch.Draw(TEXTURE, contentArea, COLOR * TRANSPARENCY);
        }
    }
}
