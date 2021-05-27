using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OctoAwesome.Definitions;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;
using OctoAwesome.Threading;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Manager für die Weltelemente im Spiel.
    /// </summary>
    public class ResourceManager : IResourceManager
    {
        public Player CurrentPlayer
        {
<<<<<<< HEAD
            get => _player ?? (_player = LoadPlayer(""));
            private set => _player = value;
        }

        public IUpdateHub UpdateHub { get; private set; }

        private readonly bool _disablePersistence = false;
        private readonly IPersistenceManager _persistenceManager = null;
        private readonly ILogger _logger;
        private readonly List<IMapPopulator> _populators = null;
        private Player _player;
        private readonly LockSemaphore _semaphoreSlim;

        /// <summary>
        /// Das aktuell geladene Universum.
        /// </summary>
        public IUniverse CurrentUniverse { get; private set; }

        public IDefinitionManager DefinitionManager { get; private set; }
        public ConcurrentDictionary<int, IPlanet> Planets { get; }

        private readonly IExtensionResolver _extensionResolver;

        private readonly CountedScopeSemaphore _loadingSemaphore;
        private CancellationToken _currentToken;
        private CancellationTokenSource _tokenSource;
=======
            get
            {
                if (player == null)
                    player = LoadPlayer("");

                return player;
            }
            private set => player = value;
        }

        public IUpdateHub UpdateHub { get; private set; }

        private readonly bool disablePersistence = false;
        private readonly IPersistenceManager persistenceManager = null;
        private readonly ILogger logger;
        private readonly List<IMapPopulator> populators = null;
        private Player player;
        private readonly LockSemaphore semaphoreSlim;

        /// <summary>
        /// Das aktuell geladene Universum.
        /// </summary>
        public IUniverse CurrentUniverse { get; private set; }

        public IDefinitionManager DefinitionManager { get; private set; }
        public ConcurrentDictionary<int, IPlanet> Planets { get; }

        private readonly IExtensionResolver extensionResolver;

        private readonly CountedScopeSemaphore loadingSemaphore;
        private CancellationToken currentToken;
        private CancellationTokenSource tokenSource;
>>>>>>> feature/performance

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="extensionResolver">ExetnsionResolver</param>
        /// <param name="definitionManager">DefinitionManager</param>
        /// <param name="settings">Einstellungen</param>
        public ResourceManager(IExtensionResolver extensionResolver, IDefinitionManager definitionManager, ISettings settings, IPersistenceManager persistenceManager)
        {
            _semaphoreSlim = new LockSemaphore(1, 1);
            _loadingSemaphore = new CountedScopeSemaphore();
            _extensionResolver = extensionResolver;
            DefinitionManager = definitionManager;
            _persistenceManager = persistenceManager;

            _logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(ResourceManager));

            _populators = extensionResolver.GetMapPopulator().OrderBy(p => p.Order).ToList();

            Planets = new ConcurrentDictionary<int, IPlanet>();

            bool.TryParse(settings.Get<string>("DisablePersistence"), out _disablePersistence);
        }

<<<<<<< HEAD
        public void InsertUpdateHub(UpdateHub updateHub) => UpdateHub = updateHub;
=======
        public void InsertUpdateHub(UpdateHub updateHub)
        {
            UpdateHub = updateHub;
        }
>>>>>>> feature/performance

        /// <summary>
        /// Erzuegt ein neues Universum.
        /// </summary>
        /// <param name="name">Name des neuen Universums.</param>
        /// <param name="seed">Weltgenerator-Seed für das neue Universum.</param>
        /// <returns>Die Guid des neuen Universums.</returns>
        public Guid NewUniverse(string name, int seed)
        {
            if (CurrentUniverse != null)
                UnloadUniverse();

            using (_loadingSemaphore.EnterScope())
            {
                _tokenSource?.Dispose();
                _tokenSource = new CancellationTokenSource();
                _currentToken = _tokenSource.Token;

                Guid guid = Guid.NewGuid();
                CurrentUniverse = new Universe(guid, name, seed);
                _persistenceManager.SaveUniverse(CurrentUniverse);
                return guid;
            }
        }

        /// <summary>
        /// Gibt alle Universen zurück, die geladen werden können.
        /// </summary>
        /// <returns>Die Liste der Universen.</returns>
        public IUniverse[] ListUniverses()
        {
<<<<<<< HEAD
            var awaiter = _persistenceManager.Load(out SerializableCollection<IUniverse> universes);
=======
            var awaiter = persistenceManager.Load(out SerializableCollection<IUniverse> universes);
>>>>>>> feature/performance

            if (awaiter == null)
                return Array.Empty<IUniverse>();
            else
                awaiter.WaitOnAndRelease();

            return universes.ToArray();
        }

        /// <summary>
        /// Lädt das Universum mit der angegebenen Guid.
        /// </summary>
        /// <param name="universeId">Die Guid des Universums.</param>
        /// <returns>Das geladene Universum.</returns>
        public bool TryLoadUniverse(Guid universeId)
        {
            // Alte Daten entfernen
            if (CurrentUniverse != null)
                UnloadUniverse();

            using (_loadingSemaphore.EnterScope())
            {
                _tokenSource?.Dispose();
                _tokenSource = new CancellationTokenSource();
                _currentToken = _tokenSource.Token;

                // Neuen Daten loaden/generieren
<<<<<<< HEAD
                var awaiter = _persistenceManager.Load(out IUniverse universe, universeId);
=======
                var awaiter = persistenceManager.Load(out IUniverse universe, universeId);
>>>>>>> feature/performance

                if (awaiter == null)
                    return false;
                else
                    awaiter.WaitOnAndRelease();

                CurrentUniverse = universe;
                if (CurrentUniverse == null)
                    throw new NullReferenceException();

                return true;
            }
        }

        /// <summary>
        /// Entlädt das aktuelle Universum.
        /// </summary>
        public void UnloadUniverse()
        {
            using (_loadingSemaphore.Wait())
                _tokenSource.Cancel();

            using (_loadingSemaphore.Wait())
            {
                if (CurrentUniverse == null)
                    return;

                _persistenceManager.SaveUniverse(CurrentUniverse);

                foreach (var planet in Planets)
                {
                    _persistenceManager.SavePlanet(CurrentUniverse.Id, planet.Value);
                    planet.Value.Dispose();
                }
<<<<<<< HEAD
                if (_persistenceManager is IDisposable disposable)
=======
                if (persistenceManager is IDisposable disposable)
>>>>>>> feature/performance
                    disposable.Dispose();
                Planets.Clear();

                CurrentUniverse = null;
                GC.Collect();
            }
        }

        /// <summary>
        /// Gibt das aktuelle Universum zurück
        /// </summary>
        /// <returns>Das gewünschte Universum, falls es existiert</returns>
<<<<<<< HEAD
        public IUniverse GetUniverse() => CurrentUniverse;
=======
        public IUniverse GetUniverse()
            => CurrentUniverse;
>>>>>>> feature/performance

        /// <summary>
        /// Löscht ein Universum.
        /// </summary>
        /// <param name="id">Die Guid des Universums.</param>
        public void DeleteUniverse(Guid id)
        {
            if (CurrentUniverse != null && CurrentUniverse.Id == id)
                throw new Exception("Universe is already loaded");

            _persistenceManager.DeleteUniverse(id);
        }

        /// <summary>
        /// Gibt den Planeten mit der angegebenen ID zurück
        /// </summary>
        /// <param name="id">Die Planteten-ID des gewünschten Planeten</param>
        /// <returns>Der gewünschte Planet, falls er existiert</returns>
        public IPlanet GetPlanet(int id)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            using (_semaphoreSlim.Wait())
            using (_loadingSemaphore.EnterScope())
            {
                _currentToken.ThrowIfCancellationRequested();

                if (!Planets.TryGetValue(id, out IPlanet planet))
                {
                    // Versuch vorhandenen Planeten zu laden
                    var awaiter = _persistenceManager.Load(out planet, CurrentUniverse.Id, id);

                    if (awaiter == null)
                    {
                        // Keiner da -> neu erzeugen
<<<<<<< HEAD
                        var rand = new Random(CurrentUniverse.Seed + id);
                        var generators = _extensionResolver.GetMapGenerator().ToArray();
                        var index = rand.Next(generators.Length - 1);
                        var generator = generators[index];
=======
                        Random rand = new Random(CurrentUniverse.Seed + id);
                        var generators = extensionResolver.GetMapGenerator().ToArray();
                        int index = rand.Next(generators.Length - 1);
                        IMapGenerator generator = generators[index];
>>>>>>> feature/performance
                        planet = generator.GeneratePlanet(CurrentUniverse.Id, id, CurrentUniverse.Seed + id);
                        // persistenceManager.SavePlanet(universe.Id, planet);
                    }
                    else
                    {
                        awaiter.WaitOnAndRelease();
                    }

                    Planets.TryAdd(id, planet);
                }
                return planet;
            }
        }

        /// <summary>
        /// Lädt einen Player.
        /// </summary>
        /// <param name="playername">Der Name des Players.</param>
        /// <returns></returns>
        public Player LoadPlayer(string playername)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            using (_loadingSemaphore.EnterScope())
            {
<<<<<<< HEAD
                _currentToken.ThrowIfCancellationRequested();
                var awaiter = _persistenceManager.Load(out Player player, CurrentUniverse.Id, playername);
=======
                currentToken.ThrowIfCancellationRequested();
                var awaiter = persistenceManager.Load(out Player player, CurrentUniverse.Id, playername);
>>>>>>> feature/performance

                if (awaiter == null)
                    player = new Player();
                else
                    awaiter.WaitOnAndRelease();

                return player;
            }
        }

        /// <summary>
        /// Speichert einen Player.
        /// </summary>
        /// <param name="player">Der Player.</param>
        public void SavePlayer(Player player)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            using (_loadingSemaphore.EnterScope())
                _persistenceManager.SavePlayer(CurrentUniverse.Id, player);
        }

        public IChunkColumn LoadChunkColumn(IPlanet planet, Index2 index)
        {
            // Load from disk
            Awaiter awaiter;
            IChunkColumn column11;

            do
            {
                using (_loadingSemaphore.EnterScope())
                {
                    _currentToken.ThrowIfCancellationRequested();
                    awaiter = _persistenceManager.Load(out column11, CurrentUniverse.Id, planet, index);
                    if (awaiter == null)
                    {
                        IChunkColumn column = planet.Generator.GenerateColumn(DefinitionManager, planet, new Index2(index.X, index.Y));
                        column11 = column;
                    }
                    else
                    {
                        awaiter.WaitOnAndRelease();
                    }

                    if (awaiter?.Timeouted ?? false)
                        _logger.Error("Awaiter timeout");
                }
            } while (awaiter != null && awaiter.Timeouted);

<<<<<<< HEAD
            var column00 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(-1, -1), planet.Size));
            var column10 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(0, -1), planet.Size));
            var column20 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(1, -1), planet.Size));
            var column01 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(-1, 0), planet.Size));
            var column21 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(1, 0), planet.Size));
            var column02 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(-1, 1), planet.Size));
            var column12 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(0, 1), planet.Size));
            var column22 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(1, 1), planet.Size));
=======
            IChunkColumn column00 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(-1, -1), planet.Size));
            IChunkColumn column10 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(0, -1), planet.Size));
            IChunkColumn column20 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(1, -1), planet.Size));

            IChunkColumn column01 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(-1, 0), planet.Size));
            IChunkColumn column21 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(1, 0), planet.Size));

            IChunkColumn column02 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(-1, 1), planet.Size));
            IChunkColumn column12 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(0, 1), planet.Size));
            IChunkColumn column22 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(1, 1), planet.Size));
>>>>>>> feature/performance

            // Zentrum
            if (!column11.Populated && column21 != null && column12 != null && column22 != null)
            {
                foreach (var populator in _populators)
                    populator.Populate(this, planet, column11, column21, column12, column22);

                column11.Populated = true;
                column11.FlagDirty();
                SaveChunkColumn(column11);
            }

            // Links oben
            if (column00 != null && !column00.Populated && column10 != null && column01 != null)
            {
                foreach (var populator in _populators)
                    populator.Populate(this, planet, column00, column10, column01, column11);

                column00.Populated = true;
                column00.FlagDirty();
                SaveChunkColumn(column00);
            }

            // Oben
            if (column10 != null && !column10.Populated && column20 != null && column21 != null)
            {
                foreach (var populator in _populators)
                    populator.Populate(this, planet, column10, column20, column11, column21);
                column10.Populated = true;
                column10.FlagDirty();
                SaveChunkColumn(column10);
            }

            // Links
            if (column01 != null && !column01.Populated && column02 != null && column12 != null)
            {
                foreach (var populator in _populators)
                    populator.Populate(this, planet, column01, column11, column02, column12);
                column01.Populated = true;
                column01.FlagDirty();
                SaveChunkColumn(column01);
            }

            return column11;

        }
        public void SaveChunkColumn(IChunkColumn chunkColumn)
        {
            if (_disablePersistence)
                return;

            using (_loadingSemaphore.EnterScope())
                _persistenceManager.SaveColumn(CurrentUniverse.Id, chunkColumn.Planet, chunkColumn);
        }

        public Entity LoadEntity(Guid entityId)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            using (_loadingSemaphore.EnterScope())
            {
<<<<<<< HEAD
                _currentToken.ThrowIfCancellationRequested();
                var awaiter = _persistenceManager.Load(out Entity entity, CurrentUniverse.Id, entityId);
=======
                currentToken.ThrowIfCancellationRequested();
                var awaiter = persistenceManager.Load(out Entity entity, CurrentUniverse.Id, entityId);
>>>>>>> feature/performance

                if (awaiter == null)
                    return null;
                else
                    awaiter.WaitOnAndRelease();

                return entity;
            }
        }

        public void SaveEntity(Entity entity)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            using (_loadingSemaphore.EnterScope())
            {
                if (entity is Player player)
                    SavePlayer(player);
                else
                    _persistenceManager.SaveEntity(entity, CurrentUniverse.Id);
            }
        }

        public IEnumerable<Entity> LoadEntitiesWithComponent<T>() where T : EntityComponent
        {
            using (_loadingSemaphore.EnterScope())
            {
                _currentToken.ThrowIfCancellationRequested();
                return _persistenceManager.LoadEntitiesWithComponent<T>(CurrentUniverse.Id);
            }
        }

        public IEnumerable<Guid> GetEntityIdsFromComponent<T>() where T : EntityComponent
        {
            using (_loadingSemaphore.EnterScope())
            {
                _currentToken.ThrowIfCancellationRequested();
                return _persistenceManager.GetEntityIdsFromComponent<T>(CurrentUniverse.Id);
            }
        }

        public IEnumerable<Guid> GetEntityIds()
        {
            using (_loadingSemaphore.EnterScope())
            {
                _currentToken.ThrowIfCancellationRequested();
                return _persistenceManager.GetEntityIds(CurrentUniverse.Id);
            }
        }

        public IEnumerable<(Guid Id, T Component)> GetEntityComponents<T>(IEnumerable<Guid> entityIds) where T : EntityComponent, new()
        {
            using (_loadingSemaphore.EnterScope())
            {
                _currentToken.ThrowIfCancellationRequested();
                return _persistenceManager.GetEntityComponents<T>(CurrentUniverse.Id, entityIds);
            }
        }
    }
}
