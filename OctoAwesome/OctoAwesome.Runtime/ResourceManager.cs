using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Manager für die Weltelemente im Spiel.
    /// </summary>
    public class ResourceManager : IResourceManager
    {
        public Player CurrentPlayer
        {
            get
            {
                if (_player == null)
                    _player = LoadPlayer("");

                return _player;
            }
            private set => _player = value;
        }

        private Guid DEFAULT_UNIVERSE = Guid.Parse("{3C4B1C38-70DC-4B1D-B7BE-7ED9F4B1A66D}");
        private bool _disablePersistence = false;
        private IPersistenceManager _persistenceManager = null;
        private GlobalChunkCache _globalChunkCache = null;
        private List<IMapPopulator> _populators = null;
        private Dictionary<int, IPlanet> _planets;
        private Player _player;

        /// <summary>
        /// Das aktuell geladene Universum.
        /// </summary>
        public IUniverse CurrentUniverse { get; private set; }

        public IDefinitionManager DefinitionManager { get; private set; }

        private IExtensionResolver _extensionResolver;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="extensionResolver">ExetnsionResolver</param>
        /// <param name="definitionManager">DefinitionManager</param>
        /// <param name="settings">Einstellungen</param>
        public ResourceManager(IExtensionResolver extensionResolver, IDefinitionManager definitionManager, ISettings settings, IPersistenceManager persistenceManager)
        {
            this._extensionResolver = extensionResolver;
            DefinitionManager = definitionManager;
            this._persistenceManager = persistenceManager;

            _populators = extensionResolver.GetMapPopulator().OrderBy(p => p.Order).ToList();

            _globalChunkCache = new GlobalChunkCache(
                (p, i) => LoadChunkColumn(p, i),
                (i) => GetPlanet(i),
                (p, i, c) => SaveChunkColumn(p, i, c));

            _planets = new Dictionary<int, IPlanet>();

            bool.TryParse(settings.Get<string>("DisablePersistence"), out _disablePersistence);
        }

        /// <summary>
        /// Der <see cref="IGlobalChunkCache"/>, der im Spiel verwendet werden soll.
        /// </summary>
        public IGlobalChunkCache GlobalChunkCache => _globalChunkCache;

        /// <summary>
        /// Erzuegt ein neues Universum.
        /// </summary>
        /// <param name="name">Name des neuen Universums.</param>
        /// <param name="seed">Weltgenerator-Seed für das neue Universum.</param>
        /// <returns>Die Guid des neuen Universums.</returns>
        public Guid NewUniverse(string name, int seed)
        {
            Guid guid = Guid.NewGuid();
            CurrentUniverse = new Universe(guid, name, seed);
            _persistenceManager.SaveUniverse(CurrentUniverse);
            return guid;
        }

        /// <summary>
        /// Gibt alle Universen zurück, die geladen werden können.
        /// </summary>
        /// <returns>Die Liste der Universen.</returns>
        public IUniverse[] ListUniverses()
        {
            _persistenceManager.Load(out var universes).WaitOn();
            return universes.ToArray();
        }

        /// <summary>
        /// Lädt das Universum mit der angegebenen Guid.
        /// </summary>
        /// <param name="universeId">Die Guid des Universums.</param>
        /// <returns>Das geladene Universum.</returns>
        public void LoadUniverse(Guid universeId)
        {
            // Alte Daten entfernen
            if (CurrentUniverse != null)
                UnloadUniverse();

            // Neuen Daten loaden/generieren
            _persistenceManager.Load(out var universe, universeId).WaitOn();
            CurrentUniverse = universe;

            if (CurrentUniverse == null)
                throw new Exception();
        }

        /// <summary>
        /// Entlädt das aktuelle Universum.
        /// </summary>
        public void UnloadUniverse()
        {
            _persistenceManager.SaveUniverse(CurrentUniverse);

            // Unload Chunks
            _globalChunkCache.Clear();

            foreach (var planet in _planets)
                _persistenceManager.SavePlanet(CurrentUniverse.Id, planet.Value);
            _planets.Clear();

            CurrentUniverse = null;
            GC.Collect();
        }

        /// <summary>
        /// Entlädt das aktuelle Universum
        /// </summary>
        /// <returns>Das gewünschte Universum, falls es existiert</returns>
        public IUniverse GetUniverse() => CurrentUniverse;

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

            IPlanet planet;

            if (!_planets.TryGetValue(id, out planet))
            {
                var awaiter = _persistenceManager.Load(out planet, CurrentUniverse.Id, id);

                if (awaiter == null)
                {
                    // Keiner da -> neu erzeugen
                    Random rand = new Random(CurrentUniverse.Seed + id);
                    var generators = _extensionResolver.GetMapGenerator().ToArray();
                    int index = rand.Next(generators.Length - 1);
                    IMapGenerator generator = generators[index];
                    planet = generator.GeneratePlanet(CurrentUniverse.Id, id, CurrentUniverse.Seed + id);
                    // persistenceManager.SavePlanet(universe.Id, planet);
                }
                else
                {
                    awaiter.WaitOn();
                }
                _planets.Add(id, planet);
            }
            return planet;
        }

        /// <summary>
        /// Lädt einen Player.
        /// </summary>
        /// <param name="playerName">Der Name des Players.</param>
        /// <returns></returns>
        public Player LoadPlayer(string playerName)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            _persistenceManager.Load(out var player, CurrentUniverse.Id, playerName).WaitOn();
            return player;
        }

        /// <summary>
        /// Speichert einen Player.
        /// </summary>
        /// <param name="player">Der Player.</param>
        public void SavePlayer(Player player)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            _persistenceManager.SavePlayer(CurrentUniverse.Id, player);
        }

        public IChunkColumn LoadChunkColumn(int planetId, Index2 index)
        {
            var planet = GetPlanet(planetId);

            // Load from disk
            var awaiter = _persistenceManager.Load(out var column11, CurrentUniverse.Id, planet, index);

            if (awaiter == null)
            {
                IChunkColumn column = planet.Generator.GenerateColumn(DefinitionManager, planet, new Index2(index.X, index.Y));
                column11 = column;
            } else
            {
                awaiter.WaitOn();
            }

            var column00 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(-1, -1), planet.Size));
            var column10 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(0, -1), planet.Size));
            var column20 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(1, -1), planet.Size));

            var column01 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(-1, 0), planet.Size));
            var column21 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(1, 0), planet.Size));

            var column02 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(-1, 1), planet.Size));
            var column12 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(0, 1), planet.Size));
            var column22 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(1, 1), planet.Size));

            // Zentrum
            if (!column11.Populated && column21 != null && column12 != null && column22 != null)
            {
                foreach (var populator in _populators)
                    populator.Populate(this, planet, column11, column21, column12, column22);

                column11.Populated = true;
            }

            // Links oben
            if (column00 != null && !column00.Populated && column10 != null && column01 != null)
            {
                foreach (var populator in _populators)
                    populator.Populate(this, planet, column00, column10, column01, column11);

                column00.Populated = true;
            }

            // Oben
            if (column10 != null && !column10.Populated && column20 != null && column21 != null)
            {
                foreach (var populator in _populators)
                    populator.Populate(this, planet, column10, column20, column11, column21);
                column10.Populated = true;
            }

            // Links
            if (column01 != null && !column01.Populated && column02 != null && column12 != null)
            {
                foreach (var populator in _populators)
                    populator.Populate(this, planet, column01, column11, column02, column12);
                column01.Populated = true;
            }

            return column11;
        }

        private void SaveChunkColumn(int planetId, Index2 index, IChunkColumn value)
        {
            if (!_disablePersistence && value.ChangeCounter > 0) //value.Chunks.Any(c => c.ChangeCounter > 0)
            {
                _persistenceManager.SaveColumn(CurrentUniverse.Id, planetId, value);
            }
        }

        public void SaveEntity(Entity entity)
        {
            if (entity is Player player)
                SavePlayer(player);
        }
    }
}
