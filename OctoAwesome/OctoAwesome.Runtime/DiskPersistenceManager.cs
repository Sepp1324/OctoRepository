using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using OctoAwesome.Components;
using OctoAwesome.Database;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using OctoAwesome.Serialization.Entities;

namespace OctoAwesome.Runtime
{
    /// <summary>
    ///     Persistiert Chunks auf die Festplatte.
    /// </summary>
    public class DiskPersistenceManager : IPersistenceManager, IDisposable, INotificationObserver
    {
        private const string UniverseFilename = "universe.info";

        private const string PlanetGeneratorInfo = "generator.info";

        private const string PlanetFilename = "planet.info";

        private readonly IPool<Awaiter> _awaiterPool;
        private readonly IPool<BlockChangedNotification> _blockChangedNotificationPool;
        private readonly IDisposable _chunkSubscription;
        private readonly DatabaseProvider _databaseProvider;
        private readonly IExtensionResolver _extensionResolver;
        private readonly ISettings _settings;
        private IUniverse _currentUniverse;

        private DirectoryInfo _root;

        public DiskPersistenceManager(IExtensionResolver extensionResolver, ISettings Settings, IUpdateHub updateHub)
        {
            _extensionResolver = extensionResolver;
            _settings = Settings;
            _databaseProvider = new DatabaseProvider(GetRoot(), TypeContainer.Get<ILogger>());
            _awaiterPool = TypeContainer.Get<IPool<Awaiter>>();
            _blockChangedNotificationPool = TypeContainer.Get<IPool<BlockChangedNotification>>();
            _chunkSubscription = updateHub.Subscribe(this, DefaultChannels.CHUNK);
        }

        public void Dispose()
        {
            _databaseProvider.Dispose();
            _chunkSubscription.Dispose();
        }

        public void OnCompleted() { }

        public void OnError(Exception error) => throw error;

        public void OnNext(Notification notification)
        {
            switch (notification)
            {
                case BlockChangedNotification blockChanged:
                    SaveChunk(blockChanged);
                    break;
                case BlocksChangedNotification blocksChanged:
                    SaveChunk(blocksChanged);
                    break;
            }
        }

        /// <summary>
        ///     Speichert das Universum.
        /// </summary>
        /// <param name="universe">Das zu speichernde Universum</param>
        public void SaveUniverse(IUniverse universe)
        {
            var path = Path.Combine(GetRoot(), universe.Id.ToString());
            Directory.CreateDirectory(path);
            _currentUniverse = universe;
            var file = Path.Combine(path, UniverseFilename);

            using Stream stream = File.Open(file, FileMode.Create, FileAccess.Write);
            using var zip = new GZipStream(stream, CompressionMode.Compress);
            using var writer = new BinaryWriter(zip);
            universe.Serialize(writer);
        }

        /// <summary>
        ///     Löscht ein Universum.
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        public void DeleteUniverse(Guid universeGuid)
        {
            var path = Path.Combine(GetRoot(), universeGuid.ToString());
            Directory.Delete(path, true);
        }

        /// <summary>
        ///     Speichert einen Planeten.
        /// </summary>
        /// <param name="universeGuid">Guid des Universums</param>
        /// <param name="planet">Zu speichernder Planet</param>
        public void SavePlanet(Guid universeGuid, IPlanet planet)
        {
            var path = Path.Combine(GetRoot(), universeGuid.ToString(), planet.Id.ToString());
            Directory.CreateDirectory(path);

            var generatorInfo = Path.Combine(path, PlanetGeneratorInfo);
            using (Stream stream = File.Open(generatorInfo, FileMode.Create, FileAccess.Write))
            {
                using (var bw = new BinaryWriter(stream))
                {
                    bw.Write(planet.Generator.GetType().FullName!);
                }
            }

            var file = Path.Combine(path, PlanetFilename);
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            using (var zip = new GZipStream(stream, CompressionMode.Compress))
            using (var writer = new BinaryWriter(zip))
            {
                planet.Serialize(writer);
            }
        }

        /// <summary>
        ///     Speichert eine <see cref="IChunkColumn" />.
        /// </summary>
        /// <param name="universeGuid">GUID des Universums.</param>
        /// <param name="planet">Index des Planeten.</param>
        /// <param name="column">Zu serialisierende ChunkColumn.</param>
        public void SaveColumn(Guid universeGuid, IPlanet planet, IChunkColumn column)
        {
            var chunkColumnContext = new ChunkColumnDbContext(_databaseProvider.GetDatabase<Index2Tag>(universeGuid, planet.Id, false), planet);
            chunkColumnContext.AddOrUpdate(column);
        }

        /// <summary>
        ///     Speichert einen Player
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        /// <param name="player">Der Player.</param>
        public void SavePlayer(Guid universeGuid, Player player)
        {
            var path = Path.Combine(GetRoot(), universeGuid.ToString());
            Directory.CreateDirectory(path);

            // TODO: Player Name berücksichtigen
            var file = Path.Combine(path, "player.info");
            using Stream stream = File.Open(file, FileMode.Create, FileAccess.Write);
            using var writer = new BinaryWriter(stream);
            player.Serialize(writer);
        }

        public void SaveEntity(Entity entity, Guid universe)
        {
            var context = new ComponentContainerDbContext<IEntityComponent>(_databaseProvider, universe);
            context.AddOrUpdate(entity);
        }

        /// <summary>
        ///     Gibt alle Universen zurück, die geladen werden können.
        /// </summary>
        /// <returns>Die Liste der Universen.</returns>
        public Awaiter Load(out SerializableCollection<IUniverse> universes)
        {
            var root = GetRoot();
            var awaiter = _awaiterPool.Get();
            universes = new SerializableCollection<IUniverse>();
            awaiter.Serializable = universes;
            foreach (var folder in Directory.GetDirectories(root))
            {
                var id = Path.GetFileNameWithoutExtension(folder); 

                if (!Guid.TryParse(id, out var guid)) 
                    continue;

                Load(out var universe, guid).WaitOnAndRelease();
                universes.Add(universe);
            }

            awaiter.SetResult(universes);

            return awaiter;
        }

        /// <summary>
        ///     Lädt das Universum mit der angegebenen Guid.
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        /// <returns>Das geladene Universum.</returns>
        public Awaiter Load(out IUniverse universe, Guid universeGuid)
        {
            var file = Path.Combine(GetRoot(), universeGuid.ToString(), UniverseFilename);
            universe = new Universe();
            _currentUniverse = universe;

            if (!File.Exists(file))
                return null;

            using Stream stream = File.Open(file, FileMode.Open, FileAccess.Read);
            using var zip = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new BinaryReader(zip);
            var awaiter = _awaiterPool.Get();
            universe.Deserialize(reader);
            awaiter.SetResult(universe);
            return awaiter;
        }

        /// <summary>
        ///     Lädt einen Planeten.
        /// </summary>
        /// <param name="universeGuid">Guid des Universums</param>
        /// <param name="planetId">Index des Planeten</param>
        /// <returns></returns>
        public Awaiter Load(out IPlanet planet, Guid universeGuid, int planetId)
        {
            var file = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString(), PlanetFilename);
            var generatorInfo = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString(), PlanetGeneratorInfo);
            planet = new Planet();
           
            if (!File.Exists(generatorInfo) || !File.Exists(file))
                return null;

            IMapGenerator generator = null;
            using (Stream stream = File.Open(generatorInfo, FileMode.Open, FileAccess.Read))
            {
                using (var bw = new BinaryReader(stream))
                {
                    var generatorName = bw.ReadString();
                    generator = _extensionResolver.GetMapGenerator()
                        .FirstOrDefault(g => g.GetType().FullName!.Equals(generatorName));
                }
            }

            if (generator == null)
                throw new("Unknown Generator");


            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                using (var zip = new GZipStream(stream, CompressionMode.Decompress))
                {
                    var awaiter = _awaiterPool.Get();
                    planet = generator.GeneratePlanet(zip);
                    awaiter.SetResult(planet);
                    return awaiter;
                }
            }
        }

        /// <summary>
        ///     Lädt eine <see cref="IChunkColumn" />.
        /// </summary>
        /// <param name="universeGuid">GUID des Universums.</param>
        /// <param name="planet">Index des Planeten.</param>
        /// <param name="columnIndex">Zu serialisierende ChunkColumn.</param>
        /// <returns>Die neu geladene ChunkColumn.</returns>
        public Awaiter Load(out IChunkColumn column, Guid universeGuid, IPlanet planet, Index2 columnIndex)
        {
            var chunkColumnContext = new ChunkColumnDbContext(_databaseProvider.GetDatabase<Index2Tag>(universeGuid, planet.Id, false), planet);

            column = chunkColumnContext.Get(columnIndex);

            if (column == null)
                return null;
            //var localColumn = column;

            ApplyChunkDiff(column, universeGuid, planet);

            var awaiter = _awaiterPool.Get();
            awaiter.SetResult(column);
            return awaiter;
        }

        public Awaiter Load(out Entity entity, Guid universeGuid, Guid entityId)
        {
            var entityContext = new ComponentContainerDbContext<IEntityComponent>(_databaseProvider, universeGuid);
            entity = (Entity)entityContext.Get(new GuidTag<ComponentContainer<IEntityComponent>>(entityId));

            var awaiter = _awaiterPool.Get();
            awaiter.SetResult(entity);
            return awaiter;
        }


        /// <summary>
        ///     Lädt einen Player.
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        /// <param name="playername">Der Name des Spielers.</param>
        /// <returns></returns>
        public Awaiter Load(out Player player, Guid universeGuid, string playername)
        {
            //TODO: Später durch Playername ersetzen
            var file = Path.Combine(GetRoot(), universeGuid.ToString(), "player.info");
            player = new();

            if (!File.Exists(file))
                return null;

            using Stream stream = File.Open(file, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(stream);
            try
            {
                var awaiter = _awaiterPool.Get();
                awaiter.Serializable = player;
                player.Deserialize(reader);
                awaiter.SetResult(player);
                return awaiter;
            }
            catch (Exception ex)
            {
                // File.Delete(file);
            }

            return null;
        }

        public IEnumerable<Entity> LoadEntitiesWithComponent<T>(Guid universeGuid) where T : IEntityComponent => new ComponentContainerDbContext<IEntityComponent>(_databaseProvider, universeGuid).GetComponentContainerWithComponent<T>().OfType<Entity>();

        public IEnumerable<Guid> GetEntityIdsFromComponent<T>(Guid universeGuid) where T : IEntityComponent => new ComponentContainerDbContext<IEntityComponent>(_databaseProvider, universeGuid).GetComponentContainerIdsFromComponent<T>().Select(i => i.Tag);

        public IEnumerable<Guid> GetEntityIds(Guid universeGuid) => new ComponentContainerDbContext<IEntityComponent>(_databaseProvider, universeGuid).GetAllKeys().Select(i => i.Tag);

        public IEnumerable<(Guid Id, T Component)> GetEntityComponents<T>(Guid universeGuid, Guid[] entityIds) where T : IEntityComponent, new()
        {
            foreach (var entityId in entityIds)
                yield return (entityId, new ComponentContainerComponentDbContext<T>(_databaseProvider, universeGuid).Get<T>(entityId));
        }

        private string GetRoot()
        {
            if (_root != null)
                return _root.FullName;

            var appConfig = _settings.Get<string>("ChunkRoot");

            if (!string.IsNullOrEmpty(appConfig))
            {
                _root = new(appConfig);
                if (!_root.Exists) _root.Create();
                return _root.FullName;
            }

            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _root = new(exePath + Path.DirectorySeparatorChar + "OctoMap");
            if (!_root.Exists) _root.Create();
            return _root.FullName;
        }

        private void SaveChunk(BlockChangedNotification chunkNotification)
        {
            var database = _databaseProvider.GetDatabase<ChunkDiffTag>(_currentUniverse.Id, chunkNotification.Planet, true);
            var databaseContext = new ChunkDiffDbContext(database, _blockChangedNotificationPool);
            databaseContext.AddOrUpdate(chunkNotification);
        }

        private void SaveChunk(BlocksChangedNotification chunkNotification)
        {
            var database = _databaseProvider.GetDatabase<ChunkDiffTag>(_currentUniverse.Id, chunkNotification.Planet, true);
            var databaseContext = new ChunkDiffDbContext(database, _blockChangedNotificationPool);
            databaseContext.AddOrUpdate(chunkNotification);
        }

        private void ApplyChunkDiff(IChunkColumn column, Guid universeGuid, IPlanet planet)
        {
            var database = _databaseProvider.GetDatabase<ChunkDiffTag>(universeGuid, planet.Id, true);
            var databaseContext = new ChunkDiffDbContext(database, _blockChangedNotificationPool);
            var keys = databaseContext.GetAllKeys();

            for (var i = 0; i < keys.Count; i++)
            {
                var key = keys[i];

                if (key.ChunkPositon.X != column.Index.X || key.ChunkPositon.Y != column.Index.Y) 
                    continue;

                var block = databaseContext.Get(key);
                column.Chunks[key.ChunkPositon.Z].Blocks[key.FlatIndex] = block.BlockInfo.Block;
                column.Chunks[key.ChunkPositon.Z].MetaData[key.FlatIndex] = block.BlockInfo.Meta;
            }

            if (keys.Count <= 1000) 
                return;

            SaveColumn(universeGuid, planet, column);
            databaseContext.Remove(keys);
        }
    }
}