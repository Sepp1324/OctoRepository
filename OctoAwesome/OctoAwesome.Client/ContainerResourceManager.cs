using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using System;
using System.Linq;
using System.Net;

namespace OctoAwesome.Client
{
    /// <summary>
    /// This is only temporary
    /// </summary>
    public class ContainerResourceManager : IResourceManager
    {
        public IDefinitionManager DefinitionManager => resourceManager.DefinitionManager;
        public IUniverse CurrentUniverse => resourceManager.CurrentUniverse;
        public IGlobalChunkCache GlobalChunkCache => resourceManager.GlobalChunkCache;

        public bool IsMultiplayer { get; private set; }
        public Player CurrentPlayer => resourceManager.CurrentPlayer;

        public IUpdateHub UpdateHub { get; }

        private ResourceManager resourceManager;
        private NetworkUpdateManager networkUpdateManager;

        public ContainerResourceManager()
        {
            UpdateHub = new UpdateHub();
        }

        public void CreateManager(IExtensionResolver extensionResolver, IDefinitionManager definitionManager,
            ISettings settings, bool multiplayer)
        {
            IPersistenceManager persistenceManager;

            if (resourceManager != null)
            {
                if (resourceManager.CurrentUniverse != null)
                    resourceManager.UnloadUniverse();

                resourceManager = null;
            }


            if (multiplayer)
            {
                var rawIPaddress = settings.Get<string>("server").Trim();
                string host;
                IPAddress ipAddress;
                int port = -1;

                if (!IPAddress.TryParse(rawIPaddress, out ipAddress))
                {
                    string stringIpAddress;

                    if (rawIPaddress[0] == '[') //IPv6 with Ports 
                    {
                        port = int.Parse(rawIPaddress.Split(':').Last());
                        stringIpAddress = rawIPaddress.Substring(1, rawIPaddress.IndexOf(']') - 1);
                    }
                    else if (rawIPaddress.Contains(':') && IPAddress.TryParse(rawIPaddress.Substring(0, rawIPaddress.IndexOf(':')), out ipAddress))
                    {
                        port = int.Parse(rawIPaddress.Split(':').Last());
                        stringIpAddress = ipAddress.ToString();
                    }
                    else if (rawIPaddress.Contains(':'))
                    {
                        port = int.Parse(rawIPaddress.Split(':').Last());
                        stringIpAddress = rawIPaddress.Split(':').First();
                    }
                    else
                    {
                        stringIpAddress = rawIPaddress;
                    }
                    host = stringIpAddress;
                }
                else
                {
                    host = rawIPaddress;
                }

                var client = new Network.Client();
                client.Connect(host, port > 0 ? (ushort)port : (ushort)8888);
                persistenceManager = new NetworkPersistenceManager(client, definitionManager);
                networkUpdateManager = new NetworkUpdateManager(client, UpdateHub, definitionManager);
            }
            else
            {
                persistenceManager = new DiskPersistenceManager(extensionResolver, definitionManager, settings);
            }

            resourceManager = new ResourceManager(extensionResolver, definitionManager, settings, persistenceManager);
            resourceManager.InsertUpdateHub(UpdateHub as UpdateHub);

            IsMultiplayer = multiplayer;

            if (multiplayer)
            {
                resourceManager.GlobalChunkCache.ChunkColumnChanged += (s, c) =>
                {
                    var networkPersistence = (NetworkPersistenceManager)persistenceManager;
                    networkPersistence.SendChangedChunkColumn(c);
                };
            }
        }

        public void DeleteUniverse(Guid id) => resourceManager.DeleteUniverse(id);

        public IPlanet GetPlanet(int planetId) => resourceManager.GetPlanet(planetId);

        public IUniverse GetUniverse() => resourceManager.GetUniverse();

        public IUniverse[] ListUniverses() => resourceManager.ListUniverses();

        public Player LoadPlayer(string playername) => resourceManager.LoadPlayer(playername);

        public void LoadUniverse(Guid universeId) => resourceManager.LoadUniverse(universeId);

        public Guid NewUniverse(string name, int seed) => resourceManager.NewUniverse(name, seed);

        public void SaveEntity(Entity entity) => resourceManager.SaveEntity(entity);

        public void SavePlayer(Player player) => resourceManager.SavePlayer(player);

        public void UnloadUniverse() => resourceManager.UnloadUniverse();
        public void SaveChunkColumn(IChunkColumn chunkColumn) => resourceManager.SaveChunkColumn(chunkColumn);
    }
}
