using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OctoAwesome.Components;
using OctoAwesome.Logging;
using OctoAwesome.Network.Pooling;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;

namespace OctoAwesome.Network
{
    public class NetworkPersistenceManager : IPersistenceManager, IDisposable
    {
        private readonly IPool<Awaiter> _awaiterPool;
        private readonly Client _client;
        private readonly ILogger _logger;
        private readonly PackagePool _packagePool;

        private readonly ConcurrentDictionary<uint, Awaiter> _packages;
        private readonly IDisposable _subscription;
        private readonly ITypeContainer _typeContainer;

        public NetworkPersistenceManager(ITypeContainer typeContainer, Client client)
        {
            _client = client;
            _subscription = client.Packages.Subscribe(OnNext, OnError);
            _typeContainer = typeContainer;

            _packages = new();
            _logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(NetworkPersistenceManager));
            _awaiterPool = TypeContainer.Get<IPool<Awaiter>>();
            _packagePool = TypeContainer.Get<PackagePool>();
        }

        private void OnNext(Package package)
        {
            _logger.Trace($"Package with id:{package.UId} for Command: {package.OfficialCommand}");

            switch (package.OfficialCommand)
            {
                case OfficialCommand.Whoami:
                case OfficialCommand.GetUniverse:
                case OfficialCommand.GetPlanet:
                case OfficialCommand.LoadColumn:
                case OfficialCommand.SaveColumn:
                    if (_packages.TryRemove(package.UId, out var awaiter))
                    {
                        if (awaiter.TrySetResult(package.Payload))
                            _logger.Warn($"Awaiter can not set result package {package.UId}");
                    }
                    else
                    {
                        _logger.Error($"No Awaiter found for Package: {package.UId}[{package.OfficialCommand}]");
                    }

                    break;
                default:
                    _logger.Warn($"Cannot handle Command: {package.OfficialCommand}");
                    break;
            }
        }

        private void OnError(Exception error) => _logger.Error(error.Message, error);

        public void DeleteUniverse(Guid universeGuid)
        {
            //throw new NotImplementedException();
        }

        public Awaiter Load(out SerializableCollection<IUniverse> universes) => throw new NotImplementedException();

        public Awaiter Load(out IChunkColumn column, Guid universeGuid, IPlanet planet, Index2 columnIndex)
        {
            var package = _packagePool.Get();
            package.Command = (ushort)OfficialCommand.LoadColumn;

            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(universeGuid.ToByteArray());
                binaryWriter.Write(planet.Id);
                binaryWriter.Write(columnIndex.X);
                binaryWriter.Write(columnIndex.Y);

                package.Payload = memoryStream.ToArray();
            }

            column = new ChunkColumn(planet);
            var awaiter = GetAwaiter(column, package.UId);

            _client.SendPackageAndRelease(package);

            return awaiter;
        }

        public Awaiter Load(out IPlanet planet, Guid universeGuid, int planetId)
        {
            var package = _packagePool.Get();
            package.Command = (ushort)OfficialCommand.GetPlanet;
            planet = _typeContainer.Get<IPlanet>();
            var awaiter = GetAwaiter(planet, package.UId);
            _client.SendPackageAndRelease(package);

            return awaiter;
        }

        public Awaiter Load(out Player player, Guid universeGuid, string playerName)
        {
            var playerNameBytes = Encoding.UTF8.GetBytes(playerName);

            var package = _packagePool.Get();
            package.Command = (ushort)OfficialCommand.Whoami;
            package.Payload = playerNameBytes;

            player = new();
            var awaiter = GetAwaiter(player, package.UId);
            _client.SendPackageAndRelease(package);

            return awaiter;
        }

        public Awaiter Load(out IUniverse universe, Guid universeGuid)
        {
            var package = _packagePool.Get();
            package.Command = (ushort)OfficialCommand.GetUniverse;

            universe = new Universe();
            var awaiter = GetAwaiter(universe, package.UId);
            _client.SendPackageAndRelease(package);

            return awaiter;
        }

        public Awaiter Load(out Entity entity, Guid universeGuid, Guid entityId)
        {
            entity = null;
            return null;
        }

        public IEnumerable<Entity> LoadEntitiesWithComponent<T>(Guid universeGuid) where T : IEntityComponent => Array.Empty<Entity>();

        public IEnumerable<Guid> GetEntityIdsFromComponent<T>(Guid universeGuid) where T : IEntityComponent => Array.Empty<Guid>();

        public IEnumerable<Guid> GetEntityIds(Guid universeGuid) => Array.Empty<Guid>();

        public IEnumerable<(Guid Id, T Component)> GetEntityComponents<T>(Guid universeGuid, Guid[] entityIds) where T : IEntityComponent, new() => Array.Empty<(Guid, T)>();

        public void SaveColumn(Guid universeGuid, IPlanet planet, IChunkColumn column)
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

        public void SaveEntity(Entity entity, Guid universe) { }

        private Awaiter GetAwaiter(ISerializable serializable, uint packageUId)
        {
            var awaiter = _awaiterPool.Get();
            awaiter.Serializable = serializable;

            if (!_packages.TryAdd(packageUId, awaiter))
                _logger.Error($"Awaiter for package {packageUId} could not be added");

            return awaiter;
        }

        public void SendChangedChunkColumn(IChunkColumn chunkColumn)
        {
            //var package = new Package((ushort)OfficialCommand.SaveColumn, 0);

            //using (var ms = new MemoryStream())
            //using (var bw = new BinaryWriter(ms))
            //{
            //    chunkColumn.Serialize(bw, definitionManager);
            //    package.Payload = ms.ToArray();
            //}


            //client.SendPackage(package);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _subscription?.Dispose();
            _typeContainer?.Dispose();
        }
    }
}