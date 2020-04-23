using Microsoft.Xna.Framework;
using OctoAwesome.Runtime;

namespace OctoAwesome.Client.Components
{
    internal sealed class PlayerProjectionComponent : GameComponent, IPlayerController
    {
        private World world;
        
        public World World { get; private set; }

        public ActorHost Player { get { return World.Player; } }

        public PlayerProjectionComponent(Game game, InputComponent input) : base(game)
        {
            World = new World(input);
        }

        public override void Update(GameTime gameTime)
        {
            World.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            World.Save();

            base.Dispose(disposing);
        }

        public Coordinate Position => throw new System.NotImplementedException();

        public float Radius => throw new System.NotImplementedException();

        public float Angle => throw new System.NotImplementedException();

        public float Height => throw new System.NotImplementedException();

        public bool OnGround => throw new System.NotImplementedException();

        public float Tilt => throw new System.NotImplementedException();

        public Vector2 Move { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public Vector2 Head { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void Jump()
        {
            throw new System.NotImplementedException();
        }

        public void Interact()
        {
            throw new System.NotImplementedException();
        }

        public void Apply()
        {
            throw new System.NotImplementedException();
        }
    }
}