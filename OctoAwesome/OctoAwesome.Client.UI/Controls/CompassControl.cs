﻿using System;
using engenious;
using engenious.Graphics;
using engenious.UI;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

namespace OctoAwesome.UI.Controls
{
    public class CompassControl : Control
    {
        private readonly AssetComponent assets;
        private readonly Texture2D compassTexture;

        public CompassControl(BaseScreenComponent screenManager, AssetComponent assets, HeadComponent headComponent) :
            base(screenManager)
        {
            this.assets = assets;

            HeadComponent = headComponent;
            Padding = Border.All(7);

            var background = assets.LoadTexture("buttonLong_brown_pressed");
            Background = NineTileBrush.FromSingleTexture(background, 7, 7);
            compassTexture = assets.LoadTexture(GetType(), "compass");
        }

        public HeadComponent HeadComponent { get; set; }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (HeadComponent is null || !assets.Ready)
                return;

            var compassValue = HeadComponent.Angle / (float)(2 * Math.PI);
            compassValue %= 1f;
            if (compassValue < 0)
                compassValue += 1f;

            var offset = (int)(compassTexture.Width * compassValue);
            offset -= contentArea.Width / 2;
            var offsetY = (compassTexture.Height - contentArea.Height) / 2;

            batch.Draw(compassTexture,
                new Rectangle(contentArea.X, contentArea.Y - offsetY, contentArea.Width, contentArea.Height),
                new Rectangle(offset, 0, contentArea.Width, contentArea.Height + offsetY), Color.White * alpha);
        }
    }
}