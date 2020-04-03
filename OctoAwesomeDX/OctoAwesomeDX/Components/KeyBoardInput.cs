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

        public bool Interact { get; private set; }

        public void Update()
        {
            KeyboardState keyBoardState = Keyboard.GetState();

            Interact = keyBoardState.IsKeyDown(Keys.Space);
            Left = keyBoardState.IsKeyDown(Keys.Left);
            Right = keyBoardState.IsKeyDown(Keys.Right);
            Down = keyBoardState.IsKeyDown(Keys.Down);
            Up = keyBoardState.IsKeyDown(Keys.Up);
        }
    }
}
