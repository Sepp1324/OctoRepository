using engenious;
using OctoAwesome.Runtime;
using System;
using System.Threading;

namespace OctoAwesome.Network
{
    public class SimulationManager
    {
        public bool IsRunning { get; private set; }

        public Simulation Simulation
        {
            get
            {
                lock (_mainLock)
                    return _simulation;
            }
            set
            {
                lock (_mainLock)
                    _simulation = value;
            }
        }

        public GameTime GameTime { get; private set; }

        public ResourceManager ResourceManager { get; private set; }

        public GameService Service { get; }

        private Simulation _simulation;
        private readonly ExtensionLoader _extensionLoader;

        private readonly ISettings _settings;
        private readonly UpdateHub _updateHub;
        private readonly Thread _backgroundThread;
        private readonly object _mainLock;

        public SimulationManager(ISettings settings, UpdateHub updateHub)
        {
            _mainLock = new object();
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

            _extensionLoader = TypeContainer.Get<ExtensionLoader>();
            _extensionLoader.LoadExtensions();

            ResourceManager = TypeContainer.Get<ResourceManager>();
            ResourceManager.InsertUpdateHub(updateHub);

            Service = new GameService(ResourceManager);
            _simulation = new Simulation(ResourceManager, _extensionLoader, Service)
            {
                IsServerSide = true
            };
            _backgroundThread = new Thread(SimulationLoop)
            {
                Name = "Simulation Loop",
                IsBackground = true
            };
        }

        public void Start()
        {
            IsRunning = true;
            GameTime = new GameTime();

            var universe = _settings.Get<string>("LastUniverse");

            if (string.IsNullOrWhiteSpace(universe))
            {
                var guid = _simulation.NewGame("melmack", new Random().Next().ToString());
                _settings.Set("LastUniverse", guid.ToString());
            }
            else
            {
                _simulation.LoadGame(new Guid(universe));
            }

            _backgroundThread.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            _simulation.ExitGame();
            _backgroundThread.Abort();
        }

        public IUniverse GetUniverse()
            => ResourceManager.CurrentUniverse;

        public IUniverse NewUniverse()
        {
            throw new NotImplementedException();
        }

        public IPlanet GetPlanet(int planetId)
        {
            var planet = ResourceManager.GetPlanet(planetId);
            planet.UpdateHub = _updateHub;
            return planet;
        }

        public IChunkColumn LoadColumn(IPlanet planet, Index2 index2) => ResourceManager.LoadChunkColumn(planet, index2);

        public IChunkColumn LoadColumn(int planetId, Index2 index2) => LoadColumn(GetPlanet(planetId), index2);

        private void SimulationLoop()
        {
            while (IsRunning)
                Simulation.Update(GameTime);
        }
    }
}
