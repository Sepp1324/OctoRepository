using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;
using OctoAwesome.Threading;

namespace OctoAwesome.Runtime
{
    /// <summary>
    ///     Manager für die Weltelemente im Spiel.
    /// </summary>
    public class ResourceManager : IResourceManager
    {
        private readonly bool _disablePersistence;

        private readonly IExtensionResolver _extensionResolver;

        private readonly CountedScopeSemaphore _loadingSemaphore;
        private readonly ILogger _logger;
        private readonly IPersistenceManager _persistenceManager;
        private readonly List<IMapPopulator> _populators;
        private readonly LockSemaphore _semaphoreSlim;
        private CancellationToken _currentToken;
        private Player _player;
        private CancellationTokenSource _tokenSource;

        /// <summary>
        ///     Konstruktor
        /// </summary>
        /// <param name="extensionResolver">ExetnsionResolver</param>
        /// <param name="definitionManager">DefinitionManager</param>
        /// <param name="settings">Einstellungen</param>
        public ResourceManager(IExtensionResolver extensionResolver, IDefinitionManager definitionManager,
            ISettings settings, IPersistenceManager persistenceManager)
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

        public Player CurrentPlayer
        {
            get => _player ??= LoadPlayer("");
            private set => _player = value;
        }

        public IUpdateHub UpdateHub { get; private set; }

        /// <summary>
        ///     Das aktuell geladene Universum.
        /// </summary>
        public IUniverse CurrentUniverse { get; private set; }

        public IDefinitionManager DefinitionManager { get; }
        public ConcurrentDictionary<int, IPlanet> Planets { get; }

        /// <summary>
        ///     Erzuegt ein neues Universum.
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

                var guid = Guid.NewGuid();
                CurrentUniverse = new Universe(guid, name, seed);
                _persistenceManager.SaveUniverse(CurrentUniverse);
                return guid;
            }
        }

        /// <summary>
        ///     Gibt alle Universen zurück, die geladen werden können.
        /// </summary>
        /// <returns>Die Liste der Universen.</returns>
        public IUniverse[] ListUniverses()
        {
            var awaiter = _persistenceManager.Load(out var universes);

            if (awaiter == null)
                return Array.Empty<IUniverse>();

            awaiter.WaitOnAndRelease();

            return universes.ToArray();
        }

        /// <summary>
        ///     Lädt das Universum mit der angegebenen Guid.
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
                var awaiter = _persistenceManager.Load(out var universe, universeId);

                if (awaiter == null)

                    return false;
                awaiter.WaitOnAndRelease();

                CurrentUniverse = universe;

                if (CurrentUniverse == null)
                    throw new NullReferenceException();

                return true;
            }
        }

        /// <summary>
        ///     Entlädt das aktuelle Universum.
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

                if (_persistenceManager is IDisposable disposable)
                    disposable.Dispose();
                Planets.Clear();

                CurrentUniverse = null;
                GC.Collect();
            }
        }

        /// <summary>
        ///     Gibt das aktuelle Universum zurück
        /// </summary>
        /// <returns>Das gewünschte Universum, falls es existiert</returns>
        public IUniverse GetUniverse() => CurrentUniverse;

        /// <summary>
        ///     Löscht ein Universum.
        /// </summary>
        /// <param name="id">Die Guid des Universums.</param>
        public void DeleteUniverse(Guid id)
        {
            if (CurrentUniverse != null && CurrentUniverse.Id == id)
                throw new("Universe is already loaded");

            _persistenceManager.DeleteUniverse(id);
        }

        /// <summary>
        ///     Gibt den Planeten mit der angegebenen ID zurück
        /// </summary>
        /// <param name="id">Die Planteten-ID des gewünschten Planeten</param>
        /// <returns>Der gewünschte Planet, falls er existiert</returns>
        public IPlanet GetPlanet(int id)
        {
            if (CurrentUniverse == null)
                throw new("No Universe loaded");

            using (_semaphoreSlim.Wait())
            using (_loadingSemaphore.EnterScope())
            {
                _currentToken.ThrowIfCancellationRequested();

                if (!Planets.TryGetValue(id, out var planet))
                {
                    // Versuch vorhandenen Planeten zu laden
                    var awaiter = _persistenceManager.Load(out planet, CurrentUniverse.Id, id);

                    if (awaiter == null)
                    {
                        // Keiner da -> neu erzeugen
                        var rand = new Random(CurrentUniverse.Seed + id);
                        var generators = _extensionResolver.GetMapGenerator().ToArray();
                        var index = rand.Next(generators.Length - 1);
                        var generator = generators[index];
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
        ///     Lädt einen Player.
        /// </summary>
        /// <param name="playerName">Der Name des Players.</param>
        /// <returns></returns>
        public Player LoadPlayer(string playerName)
        {
            if (CurrentUniverse == null)
                throw new("No Universe loaded");

            using (_loadingSemaphore.EnterScope())
            {
                _currentToken.ThrowIfCancellationRequested();
                var awaiter = _persistenceManager.Load(out var player, CurrentUniverse.Id, playerName);

                if (awaiter == null)
                    player = new Player();
                else
                    awaiter.WaitOnAndRelease();

                return player;
            }
        }

        /// <summary>
        ///     Speichert einen Player.
        /// </summary>
        /// <param name="player">Der Player.</param>
        public void SavePlayer(Player player)
        {
            if (CurrentUniverse == null)
                throw new("No Universe loaded");

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
                        var column =
                            planet.Generator.GenerateColumn(DefinitionManager, planet, new Index2(index.X, index.Y));
                        column11 = column;
                    }
                    else
                    {
                        awaiter.WaitOnAndRelease();
                    }

                    if (awaiter?.Timeouted ?? false)
                        _logger.Error("Awaiter timeout");
                }
            } while (awaiter is { Timeouted: true });

            var column00 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(-1, -1), planet.Size));
            var column10 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(0, -1), planet.Size));
            var column20 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(1, -1), planet.Size));

            var column01 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(-1, 0), planet.Size));
            var column21 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(1, 0), planet.Size));

            var column02 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(-1, 1), planet.Size));
            var column12 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(0, 1), planet.Size));
            var column22 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(1, 1), planet.Size));

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
            if (column00 is { Populated: false } && column10 != null && column01 != null)
            {
                foreach (var populator in _populators)
                    populator.Populate(this, planet, column00, column10, column01, column11);

                column00.Populated = true;
                column00.FlagDirty();
                SaveChunkColumn(column00);
            }

            // Oben
            if (column10 is { Populated: false } && column20 != null && column21 != null)
            {
                foreach (var populator in _populators)
                    populator.Populate(this, planet, column10, column20, column11, column21);

                column10.Populated = true;
                column10.FlagDirty();
                SaveChunkColumn(column10);
            }

            // Links
            if (column01 is { Populated: false } && column02 != null && column12 != null)
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
                throw new("No Universe loaded");

            using (_loadingSemaphore.EnterScope())
            {
                _currentToken.ThrowIfCancellationRequested();
                var awaiter = _persistenceManager.Load(out var entity, CurrentUniverse.Id, entityId);

                if (awaiter == null)
                    return null;
                awaiter.WaitOnAndRelease();

                return entity;
            }
        }

        public void SaveEntity(Entity entity)
        {
            if (CurrentUniverse == null)
                throw new("No Universe loaded");

            using (_loadingSemaphore.EnterScope())
            {
                if (entity is Player player)
                    SavePlayer(player);
                else
                    _persistenceManager.SaveEntity(entity, CurrentUniverse.Id);
            }
        }

        public IEnumerable<Entity> LoadEntitiesWithComponent<T>() where T : IEntityComponent
        {
            using (_loadingSemaphore.EnterScope())
            {
                _currentToken.ThrowIfCancellationRequested();
                return _persistenceManager.LoadEntitiesWithComponent<T>(CurrentUniverse.Id);
            }
        }

        public IEnumerable<Guid> GetEntityIdsFromComponent<T>() where T : IEntityComponent
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

        public (Guid Id, T Component)[] GetEntityComponents<T>(Guid[] entityIds) where T : IEntityComponent, new()
        {
            using (_loadingSemaphore.EnterScope())
            {
                _currentToken.ThrowIfCancellationRequested();
                return _persistenceManager.GetEntityComponents<T>(CurrentUniverse.Id, entityIds).ToArray(); //Hack wird noch geänder
            }
        }

        public void InsertUpdateHub(UpdateHub updateHub) => UpdateHub = updateHub;
    }
}