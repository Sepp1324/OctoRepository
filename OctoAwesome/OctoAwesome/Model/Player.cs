using OctoAwesome.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    internal sealed class Player
    {
        private Input input;

        public readonly float MAXSPEED = 100f;

        public PointF Position { get; set; }

        public Player(Input input)
        {
            this.input = input;
        }

        public void Update(TimeSpan frameTime)
        {
            float x = (input.Left ? -1f : 0f) + (input.Right ? 1f : 0f);
            float y = (input.Up ? -1f : 0f) + (input.Down ? 1f : 0f);

            Position = new Point(Position.X + (int)(x * frameTime.TotalSeconds), Position.Y + (int)(y * frameTime.TotalSeconds));
        }
    }
}
