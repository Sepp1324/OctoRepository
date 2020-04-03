using Microsoft.Xna.Framework;
using OctoAwesome.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesomeDX.Components
{
    internal sealed class WorldComponent : GameComponent
    {
        public OctoAwesome.Model.Game World { get; private set; }

        public WorldComponent(Game game, InputComponent input) : base(game)
        {
            World = new OctoAwesome.Model.Game(input);
        }

        public override void Update(GameTime gameTime)
        {
            World.Update(gameTime);
        }
    }
}
