using engenious;
using engenious.Graphics;
using engenious.UI;
using OctoAwesome.Client.Components;

namespace OctoAwesome.Client.Controls
{
    internal class CrosshairControl : Control
    {
        private readonly AssetComponent _assets;
        private readonly Color _color;
        private readonly Texture2D _texture;
        private readonly float _transparency;

        public CrosshairControl(ScreenComponent manager) : base(manager)
        {
            _assets = manager.Game.Assets;

            _transparency = 0.5f;
            _color = Color.White;

            _texture = _assets.LoadTexture(GetType(), "octocross");
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (!_assets.Ready)
                return;

            batch.Draw(_texture, contentArea, _color * _transparency);
        }
    }
}