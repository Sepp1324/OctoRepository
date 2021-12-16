using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;

namespace OctoAwesome.Client
{
    /// <summary>
    ///     This is only temporary
    /// </summary>
    public class ContainerResourceManager : IResourceManager, IDisposable
    {
        private readonly IDefinitionManager definitionManager;

        private readonly IExtensionResolver extensionResolver;
        private readonly ISettings settings;
        private readonly ITypeContainer typeContainer;
        private NetworkUpdateManager networkUpdateManager;

        private ResourceManager resourceManager;

        public ContainerResourceManager(ITypeContainer typeContainer, IUpdateHub updateHub,
            IExtensionResolver extensionResolver, IDefinitionManager definitionManager, ISettings settings)
        {
            UpdateHub = updateHub;
            this.typeContainer = typeContainer;
            this.extensionResolver = extensionResolver;
            this.definitionManager = definitionManager;
            this.settings = settings;
        }

        public bool IsMultiplayer { get; private set; }

        public void Dispose()
        {
            if (resourceManager is IDisposable disposable)
                disposable.Dispose();
        }

        public IDefinitionManager DefinitionManager => resourceManager.DefinitionManager;
        public IUniverse CurrentUniverse => resourceManager.CurrentUniverse;
        public Player CurrentPlayer => resourceManager.CurrentPlayer;

        public IUpdateHub UpdateHub { get; }

        public ConcurrentDictionary<int, IPlanet> Planets => resourceManager.Planets;

        public void DeleteUniverse(Guid id)
        {
            resourceManager.DeleteUniverse(id);
        }

        public IPlanet GetPlanet(int planetId)
        {
            var planet = resourceManager.GetPlanet(planetId);
            planet.UpdateHub = UpdateHub;
            return planet;
        }

        public IUniverse GetUniverse()
        {
            return resourceManager.GetUniverse();
        }

        public IUniverse[] ListUniverses()
        {
            return resourceManager.ListUniverses();
        }

        public Player LoadPlayer(string playername)
        {
            return resourceManager.LoadPlayer(playername);
        }

        public bool TryLoadUniverse(Guid universeId)
        {
            return resourceManager.TryLoadUniverse(universeId);
        }

        public Guid NewUniverse(string name, int seed)
        {
            return resourceManager.NewUniverse(name, seed);
        }

        public void SaveEntity(Entity entity)
        {
            resourceManager.SaveEntity(entity);
        }

        public void SavePlayer(Player player)
        {
            resourceManager.SavePlayer(player);
        }

        public void UnloadUniverse()
        {
            resourceManager.UnloadUniverse();
        }

        public void SaveChunkColumn(IChunkColumn chunkColumn)
        {
            resourceManager.SaveChunkColumn(chunkColumn);
        }

        public IChunkColumn LoadChunkColumn(IPlanet planet, Index2 index)
        {
            return resourceManager.LoadChunkColumn(planet, index);
        }

        public Entity LoadEntity(Guid entityId)
        {
            return resourceManager.LoadEntity(entityId);
        }

        public IEnumerable<Entity> LoadEntitiesWithComponent<T>() where T : IEntityComponent
        {
            return resourceManager.LoadEntitiesWithComponent<T>();
        }

        public IEnumerable<Guid> GetEntityIdsFromComponent<T>() where T : IEntityComponent
        {
            return resourceManager.GetEntityIdsFromComponent<T>();
        }

        public IEnumerable<Guid> GetEntityIds()
        {
            return resourceManager.GetEntityIds();
        }

        public (Guid Id, T Component)[] GetEntityComponents<T>(Guid[] entityIds) where T : IEntityComponent, new()
        {
            return resourceManager.GetEntityComponents<T>(entityIds);
        }

        public void CreateManager(bool multiplayer)
        {
            IPersistenceManager persistenceManager;

            if (resourceManager != null)
            {
                if (resourceManager.CurrentUniverse != null)
                    resourceManager.UnloadUniverse();

                if (resourceManager is IDisposable disposable)
                    disposable.Dispose();

                resourceManager = null;
            }


            if (multiplayer)
            {
                var rawIpAddress = settings.Get<string>("server").Trim();
                string host;
                IPAddress iPAddress;
                var port = -1;
                if (rawIpAddress[0] == '[' ||
                    !IPAddress.TryParse(rawIpAddress, out iPAddress)) //IPV4 || IPV6 without port
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
                client.Connect(host, port > 0 ? (ushort)port : (ushort)8888);
                persistenceManager = new NetworkPersistenceManager(typeContainer, client);
                networkUpdateManager = new NetworkUpdateManager(client, UpdateHub);
            }
            else
            {
                persistenceManager = new DiskPersistenceManager(extensionResolver, settings, UpdateHub);
            }

            resourceManager = new ResourceManager(extensionResolver, definitionManager, settings, persistenceManager);
            resourceManager.InsertUpdateHub(UpdateHub as UpdateHub);


            IsMultiplayer = multiplayer;

            //if (multiplayer)
            //{
            //    resourceManager.GlobalChunkCache.ChunkColumnChanged += (s, c) =>
            //    {
            //        var networkPersistence = (NetworkPersistenceManager)persistenceManager;
            //        networkPersistence.SendChangedChunkColumn(c);
            //    };
            //}
        }
    }
}