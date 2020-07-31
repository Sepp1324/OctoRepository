using engenious;
using OctoAwesome.Runtime;
using System;
using System.Threading;

namespace OctoAwesome.Network
{
    public class SimulationManager
    {
        public bool IsRunning { get; private set; }

        public IDefinitionManager DefinitionManager => _definitionManager;

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

        private Simulation _simulation;
        private ExtensionLoader _extensionLoader;
        private DefinitionManager _definitionManager;

        private ISettings _settings;

        private Thread _backgroundThread;
        private object _mainLock;

        public SimulationManager(ISettings settings)
        {
            _mainLock = new object();

            this._settings = settings; //TODO: Where are the settings?

            _extensionLoader = new ExtensionLoader(settings);
            _extensionLoader.LoadExtensions();

            _definitionManager = new DefinitionManager(_extensionLoader);

            var persistenceManager = new DiskPersistenceManager(_extensionLoader, _definitionManager, settings);

            ResourceManager = new ResourceManager(_extensionLoader, _definitionManager, settings, persistenceManager);

            //For Release resourceManager.LoadUniverse(new Guid()); 
            ResourceManager.NewUniverse("test_universe", 043848723);

            _simulation = new Simulation(ResourceManager, _extensionLoader);
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

            _simulation.NewGame("bla", 42);

            _backgroundThread.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            _simulation.ExitGame();
            _backgroundThread.Abort();
        }

        public IUniverse GetUniverse() => ResourceManager.CurrentUniverse;

        public IUniverse NewUniverse()
        {
            throw new NotImplementedException();
        }

        public IPlanet GetPlanet(int planetId) => ResourceManager.GetPlanet(planetId);

        public IChunkColumn LoadColumn(Guid guid, int planetId, Index2 index2)
            => ResourceManager.LoadChunkColumn(planetId, index2);

        private void SimulationLoop()
        {
            while (IsRunning)
            {
                Simulation.Update(GameTime);
            }
        }
    }
}
