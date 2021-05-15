using System;
using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Common;

namespace OctoAwesome.Client.Components
{
    internal sealed class SimulationComponent : GameComponent
    {
        private readonly IExtensionResolver _extensionResolver;
        private readonly IResourceManager _resourceManager;

        public Simulation Simulation { get; private set; }

        public IGameService Service { get; }

        public SimulationState State => Simulation?.State ?? SimulationState.Undefined;

        public SimulationComponent(OctoGame game, IExtensionResolver extensionResolver, IResourceManager resourceManager) : base(game)
        {
            Service = game.Service;
            _extensionResolver = extensionResolver;
            _resourceManager = resourceManager;
        }

        public Guid NewGame(string name, string seed)
        {
            if (Simulation != null)
            {
                Simulation.ExitGame();
                Simulation = null;
            }

            Simulation = new Simulation(_resourceManager, _extensionResolver, Service);
            return Simulation.NewGame(name, seed);
        }

        public void LoadGame(Guid guid)
        {
            if (Simulation != null)
            {
                Simulation.ExitGame();
                Simulation = null;
            }

            Simulation = new Simulation(_resourceManager, _extensionResolver, Service);
            Simulation.TryLoadGame(guid);
        }

        public override void Update(GameTime gameTime) => Simulation?.Update(gameTime);

        public void ExitGame()
        {
            if (Simulation == null)
                return;

            Simulation.ExitGame();
            Simulation = null;
        }

        public Player LoginPlayer(string playerName)
        {
            if (Simulation == null)
                throw new NotSupportedException();

            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            var player = _resourceManager.LoadPlayer(playerName);
            player.Components.AddComponent(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            Simulation.AddEntity(player);


            return player;
        }

        public void LogoutPlayer(Player player)
        {
            if (Simulation == null)
                throw new NotSupportedException();

            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            Simulation.RemoveEntity(player);
        }
    }
}
