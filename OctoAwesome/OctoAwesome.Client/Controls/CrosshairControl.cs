using engenious;
using engenious.Graphics;
using engenious.UI;
using OctoAwesome.Client.Components;

namespace OctoAwesome.Client.Controls
{
    class CrosshairControl : Control
    {
        readonly AssetComponent assets;
        public Color Color;
        public Texture2D Texture;
        public float Transparency;

        public CrosshairControl(ScreenComponent manager) : base(manager)
        {
            assets = manager.Game.Assets;

            Transparency = 0.5f;
            Color = Color.White;

            Texture = assets.LoadTexture(GetType(), "octocross");
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (!assets.Ready)
                return;

            batch.Draw(Texture, contentArea, Color * Transparency);
        }
    }
}