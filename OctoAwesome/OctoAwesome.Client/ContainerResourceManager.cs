using OctoAwesome.Network;
using OctoAwesome.Runtime;
using System;

namespace OctoAwesome.Client
{
    /// <summary>
    /// This is only temporary
    /// </summary>
    public class ContainerResourceManager : IResourceManager
    {
        public IDefinitionManager DefinitionManager => _resourceManager.DefinitionManager;

        public IUniverse CurrentUniverse => _resourceManager.CurrentUniverse;

        public IGlobalChunkCache GlobalChunkCache => _resourceManager.GlobalChunkCache;

        public bool IsMultiplayer { get; private set; }

        public Player CurrentPlayer => _resourceManager.CurrentPlayer;

        private ResourceManager _resourceManager;

        public void CreateManager(IExtensionResolver extensionResolver, IDefinitionManager definitionManager,
            ISettings settings, bool multiplayer)
        {
            IPersistenceManager persistenceManager;

            if (_resourceManager != null)
            {
                if (_resourceManager.CurrentUniverse != null)
                    _resourceManager.UnloadUniverse();

                _resourceManager = null;
            }


            if (multiplayer)
            {
                var host = settings.Get<string>("server").Trim().Split(':');
                persistenceManager = new NetworkPersistenceManager(host[0], host.Length > 1 ? ushort.Parse(host[1]) : (ushort)8888, definitionManager);
            }
            else
            {
                persistenceManager = new DiskPersistenceManager(extensionResolver, definitionManager, settings);
            }

            _resourceManager = new ResourceManager(extensionResolver, definitionManager, settings, persistenceManager);

            IsMultiplayer = multiplayer;

            if (multiplayer)
            {
                _resourceManager.GlobalChunkCache.ChunkColumnChanged += (s, c) =>
                {
                    var networkPersistence = (NetworkPersistenceManager)persistenceManager;
                    networkPersistence.SendChangedChunkColumn(c);
                };
            }
        }

        public void DeleteUniverse(Guid id) => _resourceManager.DeleteUniverse(id);

        public IPlanet GetPlanet(int planetId) => _resourceManager.GetPlanet(planetId);

        public IUniverse GetUniverse() => _resourceManager.GetUniverse();

        public IUniverse[] ListUniverses() => _resourceManager.ListUniverses();

        public Player LoadPlayer(string playerName) => _resourceManager.LoadPlayer(playerName);

        public void LoadUniverse(Guid universeId) => _resourceManager.LoadUniverse(universeId);

        public Guid NewUniverse(string name, int seed) => _resourceManager.NewUniverse(name, seed);

        public void SaveEntity(Entity entity) => _resourceManager.SaveEntity(entity);

        public void SavePlayer(Player player) => _resourceManager.SavePlayer(player);

        public void UnloadUniverse() => _resourceManager.UnloadUniverse();

        public void SaveChunkColumn(IChunkColumn chunkColumn) => _resourceManager.SaveChunkColumn(chunkColumn);
    }
}
