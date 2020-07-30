using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Persistiert Chunks auf die Festplatte.
    /// </summary>
    public class DiskPersistenceManager : IPersistenceManager
    {
        private const string UniverseFilename = "universe.info";

        private const string PlanetGeneratorInfo = "generator.info";

        private const string PlanetFilename = "planet.info";

        private const string ColumnFilename = "column_{0}_{1}.dat";

        private DirectoryInfo _root;
        private ISettings _settings;

        private IDefinitionManager _definitionManager;
        private IExtensionResolver _extensionResolver;

        public DiskPersistenceManager(IExtensionResolver extensionResolver, IDefinitionManager definitionManager, ISettings Settings)
        {
            _extensionResolver = extensionResolver;
            _definitionManager = definitionManager;
            _settings = Settings;
        }

        private string GetRoot()
        {
            if (_root != null)
                return _root.FullName;

            string appconfig = _settings.Get<string>("ChunkRoot");
            if (!string.IsNullOrEmpty(appconfig))
            {
                _root = new DirectoryInfo(appconfig);
                if (!_root.Exists) _root.Create();
                return _root.FullName;
            }
            else
            {
                var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                _root = new DirectoryInfo(exePath + Path.DirectorySeparatorChar + "OctoMap");
                if (!_root.Exists) _root.Create();
                return _root.FullName;
            }
        }

        /// <summary>
        /// Speichert das Universum.
        /// </summary>
        /// <param name="universe">Das zu speichernde Universum</param>
        public void SaveUniverse(IUniverse universe)
        {
            string path = Path.Combine(GetRoot(), universe.Id.ToString());
            Directory.CreateDirectory(path);

            string file = Path.Combine(path, UniverseFilename);
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            {
                using (GZipStream zip = new GZipStream(stream, CompressionMode.Compress))
                {
                    universe.Serialize(zip);
                }
            }
        }

        /// <summary>
        /// Löscht ein Universum.
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        public void DeleteUniverse(Guid universeGuid)
        {
            var path = Path.Combine(GetRoot(), universeGuid.ToString());
            Directory.Delete(path, true);
        }

        /// <summary>
        /// Speichert einen Planeten.
        /// </summary>
        /// <param name="universeGuid">Guid des Universums</param>
        /// <param name="planet">Zu speichernder Planet</param>
        public void SavePlanet(Guid universeGuid, IPlanet planet)
        {
            string path = Path.Combine(GetRoot(), universeGuid.ToString(), planet.Id.ToString());
            Directory.CreateDirectory(path);

            string generatorInfo = Path.Combine(path, PlanetGeneratorInfo);
            using (Stream stream = File.Open(generatorInfo, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(planet.Generator.GetType().FullName);
                }
            }

            string file = Path.Combine(path, PlanetFilename);
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            {
                using (GZipStream zip = new GZipStream(stream, CompressionMode.Compress))
                {
                    planet.Serialize(zip);
                }
            }
        }

        /// <summary>
        /// Speichert eine <see cref="IChunkColumn"/>.
        /// </summary>
        /// <param name="universeGuid">GUID des Universums.</param>
        /// <param name="planetId">Index des Planeten.</param>
        /// <param name="column">Zu serialisierende ChunkColumn.</param>
        public void SaveColumn(Guid universeGuid, int planetId, IChunkColumn column)
        {
            string path = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString());
            Directory.CreateDirectory(path);

            string file = path = Path.Combine(path, string.Format(ColumnFilename, column.Index.X, column.Index.Y));
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            {
                using (GZipStream zip = new GZipStream(stream, CompressionMode.Compress))
                {
                    column.Serialize(zip, _definitionManager);
                }
            }
        }

        /// <summary>
        /// Gibt alle Universen zurück, die geladen werden können.
        /// </summary>
        /// <returns>Die Liste der Universen.</returns>
        public Task<IUniverse[]> ListUniverses()
        {
            var root = GetRoot();
            var taskCompletionSource = new TaskCompletionSource<IUniverse[]>();
            var universes = new List<IUniverse>();

            foreach (var folder in Directory.GetDirectories(root))
            {
                var id = Path.GetFileNameWithoutExtension(folder);//folder.Replace(root + "\\", "");

                if (Guid.TryParse(id, out Guid guid))
                    universes.Add(LoadUniverse(guid).Result);
            }
            taskCompletionSource.SetResult(universes.ToArray());
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Lädt das Universum mit der angegebenen Guid.
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        /// <returns>Das geladene Universum.</returns>
        public Task<IUniverse> LoadUniverse(Guid universeGuid)
        {
            var file = Path.Combine(GetRoot(), universeGuid.ToString(), UniverseFilename);
            
            if (!File.Exists(file))
                return null;
            
            var taskCompletionSource = new TaskCompletionSource<IUniverse>();

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
                {
                    IUniverse universe = new Universe();
                    universe.Deserialize(zip);
                    taskCompletionSource.SetResult(universe);
                    return taskCompletionSource.Task;
                }
            }
        }

        /// <summary>
        /// Lädt einen Planeten.
        /// </summary>
        /// <param name="universeGuid">Guid des Universums</param>
        /// <param name="planetId">Index des Planeten</param>
        /// <returns></returns>
        public Task<IPlanet> LoadPlanet(Guid universeGuid, int planetId)
        {
            var file = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString(), PlanetFilename);
            var generatorInfo = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString(), PlanetGeneratorInfo);
            
            if (!File.Exists(generatorInfo) || !File.Exists(file))
                return null;

            IMapGenerator generator;
            
            using (Stream stream = File.Open(generatorInfo, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader bw = new BinaryReader(stream))
                {
                    var generatorName = bw.ReadString();
                    generator = _extensionResolver.GetMapGenerator().FirstOrDefault(g => g.GetType().FullName.Equals(generatorName));
                }
            }

            if (generator == null)
                throw new Exception("Unknown Generator");
            

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
                {
                    var taskCompletionSource = new TaskCompletionSource<IPlanet>();
                    taskCompletionSource.SetResult(generator.GeneratePlanet(zip));
                    return taskCompletionSource.Task;
                }
            }
        }

        /// <summary>
        /// Lädt eine <see cref="IChunkColumn"/>.
        /// </summary>
        /// <param name="universeGuid">GUID des Universums.</param>
        /// <param name="planet">Index des Planeten.</param>
        /// <param name="columnIndex">Zu serialisierende ChunkColumn.</param>
        /// <returns>Die neu geladene ChunkColumn.</returns>
        public Task<IChunkColumn> LoadColumn(Guid universeGuid, IPlanet planet, Index2 columnIndex)
        {
            var file = Path.Combine(GetRoot(), universeGuid.ToString(), planet.Id.ToString(), string.Format(ColumnFilename, columnIndex.X, columnIndex.Y));
            
            if (!File.Exists(file))
                return null;

            try
            {
                using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        var taskCompletionSource = new TaskCompletionSource<IChunkColumn>();
                        taskCompletionSource.SetResult(planet.Generator.GenerateColumn(zip, _definitionManager, planet.Id, columnIndex));
                        return taskCompletionSource.Task;
                    }
                }
            }
            catch (IOException)
            {
                try
                {
                    File.Delete(file);
                }
                catch (IOException) { }
                return null;
            }
        }

        /// <summary>
        /// Lädt einen Player.
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        /// <param name="playerName">Der Name des Spielers.</param>
        /// <returns></returns>
        public Task<Player> LoadPlayer(Guid universeGuid, string playerName)
        {
            //TODO: Später durch Playername ersetzen
            var file = Path.Combine(GetRoot(), universeGuid.ToString(), "player.info");
            
            if (!File.Exists(file))
                return null;

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    try
                    {
                        Player player = new Player();
                        player.Deserialize(reader, _definitionManager);
                        
                        var taskCompletionSource = new TaskCompletionSource<Player>();
                        taskCompletionSource.SetResult(player);
                        return taskCompletionSource.Task;
                    }
                    catch (Exception)
                    {
                        // File.Delete(file);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Speichert einen Player
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        /// <param name="player">Der Player.</param>
        public void SavePlayer(Guid universeGuid, Player player)
        {
            var path = Path.Combine(GetRoot(), universeGuid.ToString());
            Directory.CreateDirectory(path);

            // TODO: Player Name berücksichtigen
            var file = Path.Combine(path, "player.info");
            
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    player.Serialize(writer, _definitionManager);
                }
            }
        }
    }
}
