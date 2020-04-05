﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class MouseInput : IInputSet
    {
        private float mouseIntensity = 0.2f;

        private Game game;

        private bool init = false;

        public float MoveX { get; private set; }

        public float MoveY { get; private set; }

        public float HeadX { get; private set; }

        public float HeadY { get; private set; }

        public bool InteractTrigger { get; private set; }

        public bool JumpTrigger { get; set; }

        public MouseInput(Game game)
        {
            this.game = game;
        }

        public void Update()
        {
            MouseState state = Mouse.GetState();

            InteractTrigger = state.RightButton == ButtonState.Pressed;

            int centerX = game.GraphicsDevice.Viewport.Width / 2;
            int centerY = game.GraphicsDevice.Viewport.Height / 2;

            Mouse.SetPosition(centerX, centerY);

            if (init)
            {
                float deltaX = state.Position.X - centerX;
                float deltaY = state.Position.Y - centerY;

                HeadX = deltaX * mouseIntensity;
                HeadY = -deltaY * mouseIntensity;
            }
            init = true;
        }
    }
}
