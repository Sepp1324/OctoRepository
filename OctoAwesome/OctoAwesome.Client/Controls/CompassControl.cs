using System;
using engenious;
using engenious.Graphics;
using engenious.UI;
using OctoAwesome.Client.Components;

namespace OctoAwesome.Client.Controls
{
    internal class CompassControl : Control
    {
        private readonly AssetComponent _assets;
        private readonly Texture2D _compassTexture;

        public CompassControl(ScreenComponent screenManager) : base(screenManager)
        {
            _assets = screenManager.Game.Assets;

            Player = screenManager.Player;
            Padding = Border.All(7);

            var background = _assets.LoadTexture(typeof(ScreenComponent), "buttonLong_brown_pressed");
            Background = NineTileBrush.FromSingleTexture(background, 7, 7);
            _compassTexture = _assets.LoadTexture(GetType(), "compass");
        }

        private PlayerComponent Player { get; set; }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (Player == null || Player.CurrentEntity == null || !_assets.Ready)
                return;

            var compassValue = Player.CurrentEntityHead.Angle / (float) (2 * Math.PI);
            compassValue %= 1f;
            if (compassValue < 0)
                compassValue += 1f;

            var offset = (int) (_compassTexture.Width * compassValue);
            offset -= contentArea.Width / 2;
            var offsetY = (_compassTexture.Height - contentArea.Height) / 2;

            batch.Draw(_compassTexture, new Rectangle(contentArea.X, contentArea.Y - offsetY, contentArea.Width, contentArea.Height), new Rectangle(offset, 0, contentArea.Width, contentArea.Height + offsetY), Color.White * alpha);
        }
    }
}