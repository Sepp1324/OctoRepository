using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client
{
    /// <summary>
    /// This is only temporary
    /// </summary>
    public class ContainerResourceManager : IResourceManager, IDisposable
    {
        private readonly IDefinitionManager _definitionManager;
        private readonly IExtensionResolver _extensionResolver;
        private readonly ISettings _settings;
        
        private NetworkUpdateManager _networkUpdateManager;
        private ResourceManager _resourceManager;

        public ContainerResourceManager(IUpdateHub updateHub, IExtensionResolver extensionResolver,
            IDefinitionManager definitionManager, ISettings settings)
        {
            UpdateHub = updateHub;
            _extensionResolver = extensionResolver;
            _definitionManager = definitionManager;
            _settings = settings;
        }

        public bool IsMultiplayer { get; private set; }

        public void Dispose()
        {
            if (_resourceManager is IDisposable disposable)
                disposable.Dispose();
        }

        public IDefinitionManager DefinitionManager => _resourceManager.DefinitionManager;
        
        public IUniverse CurrentUniverse => _resourceManager.CurrentUniverse;
        
        public Player CurrentPlayer => _resourceManager.CurrentPlayer;

        public IUpdateHub UpdateHub { get; }

        public ConcurrentDictionary<int, IPlanet> Planets => _resourceManager.Planets;

        public void DeleteUniverse(Guid id) => _resourceManager.DeleteUniverse(id);

        public IPlanet GetPlanet(int planetId)
        {
            var planet = _resourceManager.GetPlanet(planetId);
            planet.UpdateHub = UpdateHub;
            return planet;
        }

        public IUniverse GetUniverse() => _resourceManager.GetUniverse();

        public IUniverse[] ListUniverses() => _resourceManager.ListUniverses();

        public Player LoadPlayer(string playername) => _resourceManager.LoadPlayer(playername);

        public bool TryLoadUniverse(Guid universeId) => _resourceManager.TryLoadUniverse(universeId);

        public Guid NewUniverse(string name, int seed) => _resourceManager.NewUniverse(name, seed);

        public void SaveEntity(Entity entity) => _resourceManager.SaveEntity(entity);

        public void SavePlayer(Player player) => _resourceManager.SavePlayer(player);

        public void UnloadUniverse() => _resourceManager.UnloadUniverse();

        public void SaveChunkColumn(IChunkColumn chunkColumn) => _resourceManager.SaveChunkColumn(chunkColumn);

        public IChunkColumn LoadChunkColumn(IPlanet planet, Index2 index) => _resourceManager.LoadChunkColumn(planet, index);

        public Entity LoadEntity(Guid entityId) => _resourceManager.LoadEntity(entityId);

        public IEnumerable<Entity> LoadEntitiesWithComponent<T>() where T : EntityComponent => _resourceManager.LoadEntitiesWithComponent<T>();

        public IEnumerable<Guid> GetEntityIdsFromComponent<T>() where T : EntityComponent => _resourceManager.GetEntityIdsFromComponent<T>();

        public IEnumerable<Guid> GetEntityIds() => _resourceManager.GetEntityIds();

        public (Guid Id, T Component)[] GetEntityComponents<T>(Guid[] entityIds) where T : EntityComponent, new() => _resourceManager.GetEntityComponents<T>(entityIds);

        public void CreateManager(bool multiplayer)
        {
            IPersistenceManager persistenceManager;

            if (_resourceManager != null)
            {
                if (_resourceManager.CurrentUniverse != null)
                    _resourceManager.UnloadUniverse();

                if (_resourceManager is IDisposable disposable)
                    disposable.Dispose();

                _resourceManager = null;
            }


            if (multiplayer)
            {
                var rawIpAddress = _settings.Get<string>("server").Trim();
                string host;
                IPAddress iPAddress;
                var port = -1;
                if (rawIpAddress[0] == '[' || !IPAddress.TryParse(rawIpAddress, out iPAddress)
                ) //IPV4 || IPV6 without port
                {
                    string stringIpAddress;
                    if (rawIpAddress[0] == '[') // IPV6 with Port
                    {
                        port = int.Parse(rawIpAddress.Split(':').Last());
                        stringIpAddress = rawIpAddress.Substring(1, rawIpAddress.IndexOf(']') - 1);
                    }
                    else if (rawIpAddress.Contains(':') &&
                             IPAddress.TryParse(rawIpAddress.Substring(0, rawIpAddress.IndexOf(':')),
                                 out iPAddress)) //IPV4 with Port
                    {
                        port = int.Parse(rawIpAddress.Split(':').Last());
                        stringIpAddress = iPAddress.ToString();
                    }
                    else if (rawIpAddress.Contains(':')) //Domain with Port
                    {
                        port = int.Parse(rawIpAddress.Split(':').Last());
                        stringIpAddress = rawIpAddress.Split(':').First();
                    }
                    else //Domain without Port
                    {
                        stringIpAddress = rawIpAddress;
                    }

                    host = stringIpAddress;
                }
                else
                {
                    host = rawIpAddress;
                }

                var client = new Network.Client();
                client.Connect(host, port > 0 ? (ushort) port : (ushort) 8888);
                persistenceManager = new NetworkPersistenceManager(client);
                _networkUpdateManager = new NetworkUpdateManager(client, UpdateHub);
            }
            else
            {
                persistenceManager = new DiskPersistenceManager(_extensionResolver, _settings, UpdateHub);
            }

            _resourceManager = new ResourceManager(_extensionResolver, _definitionManager, _settings, persistenceManager);
            _resourceManager.InsertUpdateHub(UpdateHub as UpdateHub);


            IsMultiplayer = multiplayer;
        }
    }
}