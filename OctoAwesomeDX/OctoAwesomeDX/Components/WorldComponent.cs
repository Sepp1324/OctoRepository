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

        public WorldComponent(Game game, InputComponent input)
            : base(game)
        {
            World = new World(input);
            SelectedBox = null;
        }

        public override void Update(GameTime gameTime)
        {
            World.Update(gameTime);
        }
    }
}
