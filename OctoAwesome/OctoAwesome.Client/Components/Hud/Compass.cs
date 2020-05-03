﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OctoAwesome.Client.Components.Hud
{
    internal class Compass : Control
    {
        private Texture2D compassTexture;

        public PlayerComponent Player { get; set; }

        public Compass(IScreenManager screenManager, PlayerComponent player) : base(screenManager)
        {
            Player = player;
        }

        public override void LoadContent()
        {
            compassTexture = ScreenManager.Content.Load<Texture2D>("Textures/compass");
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            float compassValue = Player.ActorHost.Angle / (float)(2 * Math.PI);
            compassValue %= 1f;
            if (compassValue < 0)
                compassValue += 1f;

            batch.Begin(samplerState: SamplerState.LinearWrap);

            int offset = (int)(compassTexture.Width * compassValue);
            offset -= Size.X / 2;
            int offsetY = (-compassTexture.Height - Size.Y) / 2;

            batch.Draw(compassTexture, new Rectangle(Position.X, Position.Y, Size.X, Size.Y), new Rectangle(offset, offsetY, Size.X, Size.Y), Color.White);

            batch.End();
        }
    }
}