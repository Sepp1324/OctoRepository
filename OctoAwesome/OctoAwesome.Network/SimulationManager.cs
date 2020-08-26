﻿using engenious;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class SimulationManager
    {
        public bool IsRunning { get; private set; }
        public IDefinitionManager DefinitionManager => definitionManager;

        public Simulation Simulation
        {
            get
            {
                lock (mainLock)
                    return simulation;
            }
            set
            {
                lock (mainLock)
                    simulation = value;
            }
        }

        public GameTime GameTime { get; private set; }

        public ResourceManager ResourceManager { get; private set; }

        private Simulation simulation;
        private ExtensionLoader extensionLoader;
        private DefinitionManager definitionManager;

        private ISettings settings;

        private Thread backgroundThread;
        private object mainLock;
        private IDisposable chunkSubscription;

        public SimulationManager(ISettings settings, UpdateHub updateHub)
        {
            mainLock = new object();

            this.settings = settings; //TODO: Where are the settings?

            extensionLoader = new ExtensionLoader(settings);
            extensionLoader.LoadExtensions();

            definitionManager = new DefinitionManager(extensionLoader);

            var persistenceManager = new DiskPersistenceManager(extensionLoader, definitionManager, settings);

            ResourceManager = new ResourceManager(extensionLoader, definitionManager, settings, persistenceManager);
            ResourceManager.InsertUpdateHub(updateHub);

            chunkSubscription = updateHub.Subscribe(ResourceManager.GlobalChunkCache, DefaultChannels.Chunk);
            ResourceManager.GlobalChunkCache.InsertUpdateHub(updateHub);

            simulation = new Simulation(ResourceManager, extensionLoader)
            {
                IsServerSide = true
            };
            backgroundThread = new Thread(SimulationLoop)
            {
                Name = "Simulation Loop",
                IsBackground = true
            };
        }

        public void Start()
        {
            IsRunning = true;
            GameTime = new GameTime();
            
            var universe = settings.Get<string>("LastUniverse");

            if (string.IsNullOrWhiteSpace(universe) || true) //TODO: If the load mechanism is repaired remove true
            {
                var guid = simulation.NewGame("melmak", new Random().Next());
                settings.Set("LastUniverse", guid.ToString());
            }
            else
            {
                simulation.LoadGame(new Guid(universe));
            }

            backgroundThread.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            simulation.ExitGame();
            backgroundThread.Abort();
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
