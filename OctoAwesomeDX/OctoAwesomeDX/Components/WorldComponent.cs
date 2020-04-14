using Microsoft.Xna.Framework;
using OctoAwesome.Components;
using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesomeDX.Components
{
    internal sealed class WorldComponent : GameComponent
    {
        public World World { get; private set; }

        public Vector3? SelectedBox { get; set; }

        private InputComponent input;

        public WorldComponent(Game game, InputComponent input)
            : base(game)
        {
            this.input = input;

            World = new World(input, 1);
            SelectedBox = null;
        }

        public override void Update(GameTime gameTime)
        {
            if (input.ApplyTrigger && SelectedBox.HasValue)
            {
                Index3 pos = new Index3((int)SelectedBox.Value.X, (int)SelectedBox.Value.Y, (int)SelectedBox.Value.Z);

                World.GetPlanet(0).SetBlock(pos, null, gameTime.TotalGameTime);
            }

            World.Update(gameTime);
        }
    }
}
