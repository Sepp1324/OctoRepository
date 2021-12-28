using engenious;
using engenious.Graphics;
using engenious.UI;
using OctoAwesome.UI.Components;

namespace OctoAwesome.UI.Controls
{
    public class CrosshairControl : Control
    {
        private readonly AssetComponent _assets;
        public Color Color;
        public Texture2D Texture;
        public float Transparency;

        public CrosshairControl(BaseScreenComponent manager, AssetComponent asset) : base(manager)
        {
            _assets = asset;

            Transparency = 0.5f;
            Color = Color.White;

            Texture = _assets.LoadTexture(GetType(), "octocross");
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (!_assets.Ready)
                return;

            batch.Draw(Texture, contentArea, Color * Transparency);
        }
    }
}