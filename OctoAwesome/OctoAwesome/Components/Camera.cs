using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Components
{
    internal sealed class Camera
    {
        private Game game;
        private Input input;

        public readonly float MAXSPEED = 100f;

        public Camera(Game game, Input input)
        {
            this.game = game;
            this.input = input;
        }

        public void Update(TimeSpan frameTime)
        {
            Vector2 velocity = new Vector2((input.CamLeft ? -1f : 0f) + (input.CamRight ? 1f : 0f), (input.CamUp ? -1f : 0f) + (input.CamDown ? 1f : 0f));

            velocity = velocity.Normalized();

            Center += (velocity * MAXSPEED * (float)frameTime.TotalSeconds);

            if (Center.X < 0)
                Center = new Vector2(0, Center.Y);

            if (Center.Y < 0)
                Center = new Vector2(Center.X, 0);

            if (Center.X > game.PlaygroundSize.X)
                Center = new Vector2(game.PlaygroundSize.X, Center.Y);

            if (Center.Y > game.PlaygroundSize.Y)
                Center = new Vector2(Center.X, game.PlaygroundSize.Y);
        }

        public Vector2 Center { get; set; }
    }
}
