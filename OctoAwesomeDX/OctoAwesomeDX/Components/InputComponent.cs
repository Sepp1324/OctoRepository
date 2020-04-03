using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class InputComponent : GameComponent
    {
        public bool Left { get; private set; }

        public bool Right { get; private set; }

        public bool Up { get; private set; }

        public bool Down { get; private set; }

        public bool Interact { get; set; }

        public InputComponent(Game game)
            : base(game)
        {

        }

        public override void Update(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            Interact = gamePadState.Buttons.A == ButtonState.Pressed;
            Left = gamePadState.ThumbSticks.Left.X < -0.5f;
            Right = gamePadState.ThumbSticks.Left.X > 0.5f;
            Down = gamePadState.ThumbSticks.Left.Y < -0.5f;
            Up = gamePadState.ThumbSticks.Left.Y > 0.5f;

            KeyboardState keyBoardState = Keyboard.GetState();

            Interact |= keyBoardState.IsKeyDown(Keys.Space);
            Left |= keyBoardState.IsKeyDown(Keys.Left);
            Right |= keyBoardState.IsKeyDown(Keys.Right);
            Down |= keyBoardState.IsKeyDown(Keys.Down);
            Up |= keyBoardState.IsKeyDown(Keys.Up);
        }
    }
}
