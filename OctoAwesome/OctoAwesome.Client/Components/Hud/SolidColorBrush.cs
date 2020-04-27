﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal class SolidColorBrush : Brush
    {
        private Texture2D pix;

        public Color Color { get; set; }

        public SolidColorBrush(HudComponent hud)
        {
            pix = hud.Game.Content.Load<Texture2D>("Textures/pix");
        }

        public override void Draw(SpriteBatch batch, Rectangle rectangle)
        {
            batch.Begin();

            batch.Draw(pix, rectangle, Color);

            batch.End();
        }
    }
}
