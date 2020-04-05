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
        public bool Left { get; private set; }

        public bool Right { get; private set; }

        public bool Up { get; private set; }

        public bool Down { get; private set; }

        public bool HeadLeft { get; private set; }

        public bool HeadRight { get; private set; }

        public bool HeadUp { get; private set; }

        public bool HeadDown { get; private set; }

        public bool Interact { get; private set; }

        public void Update()
        {
            KeyboardState keyBoardState = Keyboard.GetState();

            Interact = keyBoardState.IsKeyDown(Keys.Space);

            Left = keyBoardState.IsKeyDown(Keys.A);
            Right = keyBoardState.IsKeyDown(Keys.D);
            Down = keyBoardState.IsKeyDown(Keys.S);
            Up = keyBoardState.IsKeyDown(Keys.W);

            HeadLeft = keyBoardState.IsKeyDown(Keys.Left);
            HeadRight = keyBoardState.IsKeyDown(Keys.Right);
            HeadDown = keyBoardState.IsKeyDown(Keys.Down);
            HeadUp = keyBoardState.IsKeyDown(Keys.Up);
        }
    }
}
