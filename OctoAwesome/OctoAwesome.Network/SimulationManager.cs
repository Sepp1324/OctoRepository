using System;
using System.Threading;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.Definitions;
using OctoAwesome.Runtime;

namespace OctoAwesome.Network
{
    public class SimulationManager
    {
        private readonly object _mainLock;

        private readonly ISettings _settings;
        private readonly UpdateHub _updateHub;

        private Task _backgroundTask;
        private CancellationTokenSource _cancellationTokenSource;

        private Simulation _simulation;

        public SimulationManager(ISettings settings, UpdateHub updateHub)
        {
            _mainLock = new();

            _settings = settings;
            _updateHub = updateHub;


            TypeContainer.Register<ExtensionLoader>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IExtensionLoader, ExtensionLoader>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IExtensionResolver, ExtensionLoader>(InstanceBehaviour.Singleton);
            TypeContainer.Register<DefinitionManager>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IDefinitionManager, DefinitionManager>(InstanceBehaviour.Singleton);
            TypeContainer.Register<DiskPersistenceManager>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IPersistenceManager, DiskPersistenceManager>(InstanceBehaviour.Singleton);
            TypeContainer.Register<ResourceManager>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IResourceManager, ResourceManager>(InstanceBehaviour.Singleton);

            var extensionLoader = TypeContainer.Get<ExtensionLoader>();
            extensionLoader.LoadExtensions();

            ResourceManager = TypeContainer.Get<ResourceManager>();
            ResourceManager.InsertUpdateHub(updateHub);

            Service = new GameService(ResourceManager);
            _simulation = new Simulation(ResourceManager, extensionLoader, Service)
            {
                IsServerSide = true
            };
        }

        public bool IsRunning { get; private set; }

        public Simulation Simulation
        {
            get
            {
                lock (_mainLock)
                {
                    return _simulation;
                }
            }
            set
            {
                lock (_mainLock)
                {
                    _simulation = value;
                }
            }
        }

        public GameTime GameTime { get; private set; }

        public ResourceManager ResourceManager { get; }

        public GameService Service { get; }

        public void Start()
        {
            IsRunning = true;
            GameTime = new GameTime();

            _cancellationTokenSource = new();
            var token = _cancellationTokenSource.Token;
            _backgroundTask = new(SimulationLoop, token, token, TaskCreationOptions.LongRunning);

            //TODO: Load and Save logic for Server (Multiple games etc.....)
            var universe = _settings.Get<string>("LastUniverse");

            if (string.IsNullOrWhiteSpace(universe))
            {
                var guid = _simulation.NewGame("melmack", new Random().Next().ToString());
                _settings.Set("LastUniverse", guid.ToString());
            }
            else
            {
                if (!_simulation.TryLoadGame(new Guid(universe)))
                {
                    var guid = _simulation.NewGame("melmack", new Random().Next().ToString());
                    _settings.Set("LastUniverse", guid.ToString());
                }
            }

            _backgroundTask.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            _simulation.ExitGame();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        public IUniverse GetUniverse() => ResourceManager.CurrentUniverse;

        public IUniverse NewUniverse() => throw new NotImplementedException();

        public IPlanet GetPlanet(int planetId)
        {
            var planet = ResourceManager.GetPlanet(planetId);
            planet.UpdateHub = _updateHub;
            return planet;
        }

        public IChunkColumn LoadColumn(IPlanet planet, Index2 index2) => ResourceManager.LoadChunkColumn(planet, index2);

        public IChunkColumn LoadColumn(int planetId, Index2 index2) => LoadColumn(GetPlanet(planetId), index2);

        private void SimulationLoop(object state)
        {
            var token = state is CancellationToken stateToken ? stateToken : CancellationToken.None;

            while (true)
            {
                token.ThrowIfCancellationRequested();
                Simulation.Update(GameTime);
            }
        }
    }
}