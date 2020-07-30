using OctoAwesome.Basics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class NetworkPersistenceManager : IPersistenceManager
    {
        private readonly Client _client;
        private readonly IDefinitionManager _definitionManager;

        private Dictionary<uint, TaskCompletionSource<ISerializable>> _packages;

        public NetworkPersistenceManager(IDefinitionManager definitionManager)
        {
            _client = new Client();
            _client.PackageAvailable += ClientPackageAvailable;
            _definitionManager = definitionManager;
        }

        public NetworkPersistenceManager(string host, ushort port, IDefinitionManager definitionManager) :
            this(definitionManager) => _client.Connect(host, port);

        public void DeleteUniverse(Guid universeGuid)
        {
            //throw new NotImplementedException();
        }

        public Task<IUniverse[]> ListUniverses() => throw new NotImplementedException();

        public Task<IChunkColumn> LoadColumn(Guid universeGuid, IPlanet planet, Index2 columnIndex)
        {
            var package = new Package((ushort) OfficialCommands.LoadColumn, 0);

            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(universeGuid.ToByteArray());
                binaryWriter.Write(planet.Id);
                binaryWriter.Write(columnIndex.X);
                binaryWriter.Write(columnIndex.Y);

                package.Payload = memoryStream.ToArray();
            }

            _client.SendPackage(package);

            package = _client.SendPackage(package);

            using (var memoryStream = new MemoryStream(package.Payload))
            {
                var column = new ChunkColumn();
                column.Deserialize(memoryStream, _definitionManager, planet.Id, columnIndex);

                var taskCompletionSource = new TaskCompletionSource<IChunkColumn>();
                return taskCompletionSource.Task;
            }
        }

        public Task<IPlanet> LoadPlanet(Guid universeGuid, int planetId)
        {
            var package = new Package((ushort) OfficialCommands.GetPlanet, 0);
            package = _client.SendAndReceive(package);

            var planet = new ComplexPlanet();

            using (var memoryStream = new MemoryStream(package.Payload))
            using (var reader = new BinaryReader(memoryStream))
                planet.Deserialize(reader, null);

            var taskCompletionSource = new TaskCompletionSource<IPlanet>();
            return taskCompletionSource.Task;
        }

        public Task<Player> LoadPlayer(Guid universeGuid, string playerName)
        {
            var playerNameBytes = Encoding.UTF8.GetBytes(playerName);

            var package = new Package((ushort) OfficialCommands.Whoami, playerNameBytes.Length)
            {
                Payload = playerNameBytes
            };

            package = _client.SendAndReceive(package);

            var player = new Player();

            using (var ms = new MemoryStream(package.Payload))
            using (var br = new BinaryReader(ms))
            {
                player.Deserialize(br, _definitionManager);
            }

            var taskCompletionSource = new TaskCompletionSource<Player>();
            return taskCompletionSource.Task;
        }

        public Task<IUniverse> LoadUniverse(Guid universeGuid)
        {
            var package = new Package((ushort) OfficialCommands.GetUniverse, 0);
            Thread.Sleep(60);
            package = _client.SendAndReceive(package);

            var universe = new Universe();

            using (var memoryStream = new MemoryStream(package.Payload))
                universe.Deserialize(memoryStream);

            var taskCompletionSource = new TaskCompletionSource<IUniverse>();
            return taskCompletionSource.Task;
        }

        public void SaveColumn(Guid universeGuid, int planetId, IChunkColumn column)
        {
            //throw new NotImplementedException();
        }

        public void SavePlanet(Guid universeGuid, IPlanet planet)
        {
            //throw new NotImplementedException();
        }

        public void SavePlayer(Guid universeGuid, Player player)
        {
            //throw new NotImplementedException();
        }

        public void SaveUniverse(IUniverse universe)
        {
            //throw new NotImplementedException();
        }

        private void ClientPackageAvailable(object sender, Package e)
        {
        }
    }
}