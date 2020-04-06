using Microsoft.Xna.Framework.Input;
using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Components
{
    internal sealed class KeyBoardInput : IInputSet
    {
        public float MoveX { get; private set; }

        public float MoveY { get; private set; }

        public float HeadX { get; private set; }

        public float HeadY { get; private set; }

        public bool InteractTrigger { get; private set; }

        public bool JumpTrigger { get; private set; }

        public void Update()
        {
            KeyboardState keyBoardState = Keyboard.GetState();

            MoveX = 0f;
            MoveY = 0f;
            HeadX = 0f;
            HeadY = 0f;

            InteractTrigger = keyBoardState.IsKeyDown(Keys.E);
            JumpTrigger = keyBoardState.IsKeyDown(Keys.Space);

            MoveX -= (keyBoardState.IsKeyDown(Keys.A) ? 1 : 0);
            MoveX += (keyBoardState.IsKeyDown(Keys.D) ? 1 : 0);
            MoveY -= (keyBoardState.IsKeyDown(Keys.S) ? 1 : 0);
            MoveY += (keyBoardState.IsKeyDown(Keys.W) ? 1 : 0);

            HeadX -= (keyBoardState.IsKeyDown(Keys.Left) ? 1 : 0);
            HeadX += (keyBoardState.IsKeyDown(Keys.Right) ? 1 : 0);
            HeadY -= (keyBoardState.IsKeyDown(Keys.Up) ? 1 : 0);
            HeadY += (keyBoardState.IsKeyDown(Keys.Down) ? 1 : 0);
        }
    }
}
