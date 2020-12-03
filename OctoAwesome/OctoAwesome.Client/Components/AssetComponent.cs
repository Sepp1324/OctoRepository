using engenious;
using engenious.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace OctoAwesome.Client.Components
{
    internal sealed class AssetComponent : DrawableGameComponent
    {
        private Settings settings;

        public const string INFOFILENAME = "packinfo.xml";

        public const string SETTINGSKEY = "ActiveResourcePacks";

        public const string RESOURCEPATH = "Resources";

        private readonly Dictionary<string, Texture2D> _textures;
        private readonly Dictionary<string, Bitmap> _bitmaps;
        private string[] textureTypes = new string[] { "png", "jpg", "jpeg", "bmp" };
        private readonly List<ResourcePack> _loadedPacks = new List<ResourcePack>();
        private readonly List<ResourcePack> _activePacks = new List<ResourcePack>();

        /// <summary>
        /// Gibt an, ob der Asset Manager bereit zum Laden von Resourcen ist.
        /// </summary>
        public bool Ready { get; private set; }

        /// <summary>
        /// Gibt die Anzahl geladener Texturen zurück.
        /// </summary>
        public int LoadedTextures { get { return _textures.Count + _bitmaps.Count; } }

        /// <summary>
        /// Auflistung aller bekannten Resource Packs.
        /// </summary>
        public IEnumerable<ResourcePack> LoadedResourcePacks { get { return _loadedPacks.AsEnumerable(); } }

        /// <summary>
        /// Auflistung aller aktuell aktiven Resource Packs.
        /// </summary>
        public IEnumerable<ResourcePack> ActiveResourcePacks { get { return _activePacks.AsEnumerable(); } }

        public AssetComponent(OctoGame game) : base(game)
        {
            settings = game.Settings;

            Ready = false;
            _textures = new Dictionary<string, Texture2D>();
            _bitmaps = new Dictionary<string, Bitmap>();
            ScanForResourcePacks();

            // Load list of active Resource Packs
            var toLoad = new List<ResourcePack>();
          
            if (settings.KeyExists(SETTINGSKEY))
            {
                var activePackPathes = settings.Get<string>(SETTINGSKEY);
              
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

        public void ScanForResourcePacks()
        {
            _loadedPacks.Clear();
           
            if (Directory.Exists(RESOURCEPATH))
            {
                foreach (var directory in Directory.GetDirectories(RESOURCEPATH))
                {
                    var info = new DirectoryInfo(directory);
                   
                    if (File.Exists(Path.Combine(directory, INFOFILENAME)))
                    {
                        // Scan info File
                        var serializer = new XmlSerializer(typeof(ResourcePack));
                      
                        using (Stream stream = File.OpenRead(Path.Combine(directory, INFOFILENAME)))
                        {
                            var pack = (ResourcePack)serializer.Deserialize(stream);
                            pack.Path = info.FullName;
                            _loadedPacks.Add(pack);
                        }
                    }
                    else
                    {
                        var pack = new ResourcePack()
                        {
                            Path = info.FullName,
                            Name = info.Name
                        };

                        _loadedPacks.Add(pack);
                    }
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
            settings.Set(SETTINGSKEY, string.Join(";", _activePacks.Select(p => p.Path)));

            Ready = true;
        }

        public Texture2D LoadTexture(Type baseType, string key)
        {
            lock (_textures)
            {
                return Load(baseType, key, textureTypes, _textures, (stream) => Texture2D.FromStream(GraphicsDevice, stream));
            }
        }

        public Bitmap LoadBitmap(Type baseType, string key)
        {
            lock (_bitmaps)
            {
                return Load(baseType, key, textureTypes, _bitmaps, (stream) => (Bitmap)Image.FromStream(stream));
            }
        }

        public Stream LoadStream(Type baseType, string key, params string[] fileTypes)
        {
            return Load(baseType, key, fileTypes, null, (stream) =>
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

        private T Load<T>(Type baseType, string key, string[] fileTypes, Dictionary<string, T> cache, Func<Stream, T> callback)
        {
            if (baseType == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(key))
                return default(T);

            var fullKey = $"{baseType.Namespace}.{key}";

            var basefolder = baseType.Namespace.Replace('.', Path.DirectorySeparatorChar);

            // Cache fragen
            var result = default(T);
           
            if (cache != null && cache.TryGetValue(fullKey, out result))
                return result;

            // Versuche Datei zu laden
            foreach (var resourcePack in _activePacks)
            {
                var localFolder = Path.Combine(resourcePack.Path, basefolder);

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
                var assemblyName = baseType.Assembly.GetName().Name;

                // Spezialfall Client
                if (assemblyName.Equals("OctoClient"))
                    assemblyName = "OctoAwesome.Client";

                var resKey = fullKey.Replace(assemblyName, string.Format("{0}.Assets", assemblyName));
                foreach (var fileType in fileTypes)
                {
                    using (var stream = baseType.Assembly.GetManifestResourceStream(string.Format("{0}.{1}", resKey, fileType)))
                    {
                        if (stream != null)
                        {
                            result = callback(stream);
                            break;
                        }
                    }
                }
            }

            if (result == null)
            {
                // Im worstcase CheckerTex laden
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OctoAwesome.Client.Assets.FallbackTexture.png"))
                {
                    result = callback(stream);
                }
            }

            // In Cache speichern
            if (result != null && cache != null)
                cache[fullKey] = result;

            return result;
        }
    }
}
