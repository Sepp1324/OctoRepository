using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using engenious;
using engenious.Graphics;

namespace OctoAwesome.Client.Components
{
    internal sealed class AssetComponent : DrawableGameComponent
    {
        public const string InfoFilename = "packinfo.xml";

        public const string SettingsKey = "ActiveResourcePacks";

        public const string ResourcePath = "Resources";

        private readonly List<ResourcePack> _activePacks = new();

        private readonly Dictionary<string, Bitmap> _bitmaps;

        private readonly List<ResourcePack> _loadedPacks = new();
        private readonly Settings _settings;

        private readonly Dictionary<string, Texture2D> _textures;

        private readonly string[] _textureTypes = {"png", "jpg", "jpeg", "bmp"};

        public AssetComponent(OctoGame game) : base(game)
        {
            _settings = game.Settings;

            Ready = false;
            _textures = new Dictionary<string, Texture2D>();
            _bitmaps = new Dictionary<string, Bitmap>();
            ScanForResourcePacks();

            // Load list of active Resource Packs
            var toLoad = new List<ResourcePack>();
            if (_settings.KeyExists(SettingsKey))
            {
                var activePackPathes = _settings.Get<string>(SettingsKey);
                if (!string.IsNullOrEmpty(activePackPathes))
                {
                    var packPathes = activePackPathes.Split(';');
                    foreach (var packPath in packPathes)
                    {
                        var resourcePack = _loadedPacks.FirstOrDefault(p => p.Path.Equals(packPath));
                        if (resourcePack != null) toLoad.Add(resourcePack);
                    }
                }
            }

            ApplyResourcePacks(toLoad);
        }

        /// <summary>
        ///     Gibt an, ob der Asset Manager bereit zum Laden von Resourcen ist.
        /// </summary>
        public bool Ready { get; private set; }

        /// <summary>
        ///     Gibt die Anzahl geladener Texturen zurück.
        /// </summary>
        public int LoadedTextures => _textures.Count + _bitmaps.Count;

        /// <summary>
        ///     Auflistung aller bekannten Resource Packs.
        /// </summary>
        public IEnumerable<ResourcePack> LoadedResourcePacks => _loadedPacks.AsEnumerable();

        /// <summary>
        ///     Auflistung aller aktuell aktiven Resource Packs.
        /// </summary>
        public IEnumerable<ResourcePack> ActiveResourcePacks => _activePacks.AsEnumerable();

        public void ScanForResourcePacks()
        {
            _loadedPacks.Clear();

            if (Directory.Exists(ResourcePath))
                foreach (var directory in Directory.GetDirectories(ResourcePath))
                {
                    var info = new DirectoryInfo(directory);
                    if (File.Exists(Path.Combine(directory, InfoFilename)))
                    {
                        // Scan info File
                        var serializer = new XmlSerializer(typeof(ResourcePack));
                        using Stream stream = File.OpenRead(Path.Combine(directory, InfoFilename));
                        var pack = (ResourcePack) serializer.Deserialize(stream);
                        pack!.Path = info.FullName;
                        _loadedPacks.Add(pack);
                    }
                    else
                    {
                        var pack = new ResourcePack
                        {
                            Path = info.FullName,
                            Name = info.Name
                        };

                        _loadedPacks.Add(pack);
                    }
                }
        }

        public void ApplyResourcePacks(IEnumerable<ResourcePack> packs)
        {
            Ready = false;

            // Reset vorhandener Assets
            foreach (var component in Game.Components.OfType<IAssetRelatedComponent>())
                component.UnloadAssets();

            // Dispose Bitmaps
            lock (_bitmaps)
            {
                foreach (var value in _bitmaps.Values)
                    value.Dispose();
                _bitmaps.Clear();
            }

            // Dispose textures
            lock (_textures)
            {
                foreach (var value in _textures.Values)
                    value.Dispose();
                _textures.Clear();
            }

            // Set new Active Resource Packs
            _activePacks.Clear();
            foreach (var pack in packs)
                if (_loadedPacks.Contains(pack)) // Warum eigentlich keine eigenen Packs?
                    _activePacks.Add(pack);

            // Signal zum Reload senden
            foreach (var component in Game.Components.OfType<IAssetRelatedComponent>())
                component.ReloadAssets();

            // Speichern der Settings
            _settings.Set(SettingsKey, string.Join(";", _activePacks.Select(p => p.Path)));

            Ready = true;
        }

        public Texture2D LoadTexture(Type baseType, string key)
        {
            lock (_textures)
            {
                return Load(baseType, key, _textureTypes, _textures, stream => Texture2D.FromStream(GraphicsDevice, stream));
            }
        }

        public Bitmap LoadBitmap(Type baseType, string key)
        {
            lock (_bitmaps)
            {
                return Load(baseType, key, _textureTypes, _bitmaps, stream => (Bitmap) Image.FromStream(stream));
            }
        }

        public Stream LoadStream(Type baseType, string key, params string[] fileTypes)
        {
            return Load(baseType, key, fileTypes, null, stream =>
            {
                var result = new MemoryStream();
                var buffer = new byte[1024];
                var count = 0;
                do
                {
                    count = stream.Read(buffer, 0, buffer.Length);
                    result.Write(buffer, 0, count);
                } while (count != 0);

                result.Seek(0, SeekOrigin.Begin);
                return result;
            });
        }

        private T Load<T>(Type baseType, string key, string[] fileTypes, Dictionary<string, T> cache,
            Func<Stream, T> callback)
        {
            if (baseType == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(key))
                return default;

            var fullKey = $"{baseType.Namespace}.{key}";

            var baseFolder = baseType.Namespace!.Replace('.', Path.DirectorySeparatorChar);

            // Cache fragen
            var result = default(T);

            if (cache != null && cache.TryGetValue(fullKey, out result))
                return result;

            // Versuche Datei zu laden
            foreach (var resourcePack in _activePacks)
            {
                var localFolder = Path.Combine(resourcePack.Path, baseFolder);

                foreach (var fileType in fileTypes)
                {
                    var filename = Path.Combine(localFolder, string.Format("{0}.{1}", key, fileType));
                    if (File.Exists(filename))
                    {
                        using (var stream = File.Open(filename, FileMode.Open))
                        {
                            result = callback(stream);
                        }

                        break;
                    }
                }

                if (result != null)
                    break;
            }

            // Resource Fallback
            if (result == null)
            {
                var assemblyName = baseType.Assembly.GetName().Name!;

                // Spezialfall Client
                if (assemblyName.Equals("OctoClient"))
                    assemblyName = "OctoAwesome.Client";

                var resKey = fullKey.Replace(assemblyName, $"{assemblyName}.Assets");
                foreach (var fileType in fileTypes)
                    using (var stream =
                        baseType.Assembly.GetManifestResourceStream($"{resKey}.{fileType}"))
                    {
                        if (stream != null)
                        {
                            result = callback(stream);
                            break;
                        }
                    }
            }

            if (result == null)
                // Im worstcase CheckerTex laden
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OctoAwesome.Client.Assets.FallbackTexture.png"))
                {
                    result = callback(stream);
                }

            // In Cache speichern
            if (result != null && cache != null)
                cache[fullKey] = result;

            return result;
        }
    }
}