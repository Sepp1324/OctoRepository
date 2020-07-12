﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Components
{
    internal sealed class AssetComponent : DrawableGameComponent
    {
        public const string INFOFILENAME = "packinfo.xml";

        Dictionary<string, Texture2D> textures;

        Dictionary<string, Bitmap> bitmaps;

        string[] textureTypes = new string[] { "png", "jpg", "jpeg", "bmp" };

        List<ResourcePack> resourcePacks = new List<ResourcePack>();

        List<ResourcePack> activePacks = new List<ResourcePack>();

        /// <summary>
        /// Gibt die Anzahl geladener Texturen zurück.
        /// </summary>
        public int LoadedTextures { get { return textures.Count + bitmaps.Count; } }

        /// <summary>
        /// Auflistung aller bekannten Resource Packs.
        /// </summary>
        public IEnumerable<ResourcePack> LoadedResourcePacks { get { return resourcePacks.AsEnumerable(); } }

        public AssetComponent(OctoGame game) : base(game)
        {
            textures = new Dictionary<string, Texture2D>();
            bitmaps = new Dictionary<string, Bitmap>();
            ScanForResourcePacks();
        }

        public void ScanForResourcePacks()
        {
            resourcePacks.Clear();
            foreach (var directory in Directory.GetDirectories("Resources"))
            {
                DirectoryInfo info = new DirectoryInfo(directory);
                if (File.Exists(Path.Combine(directory, INFOFILENAME)))
                {
                    // TODO: Scan info File
                }
                else
                {
                    ResourcePack pack = new ResourcePack()
                    {
                        Path = info.FullName,
                        Name = info.Name
                    };

                    resourcePacks.Add(pack);
                }
            }
        }

        public Texture2D LoadTexture(Type baseType, string key)
        {
            return Load(baseType, key, textures, (stream) => Texture2D.FromStream(GraphicsDevice, stream));
        }

        public Bitmap LoadBitmap(Type baseType, string key)
        {
            return Load(baseType, key, bitmaps, (stream) => (Bitmap)Image.FromStream(stream));
        }

        private T Load<T>(Type baseType, string key, Dictionary<string, T> cache, Func<Stream, T> callback)
        {
            if (baseType == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException();

            string fullkey = string.Format("{0}.{1}", baseType.Namespace, key);

            // OctoAwesome.Basics.Assets.Definitions.blocks.ice

            string basefolder = baseType.Namespace.
                // Replace("OctoAwesome.", "").
                Replace('.', Path.DirectorySeparatorChar);

            // Cache fragen
            T result = default(T);
            if (cache.TryGetValue(fullkey, out result))
                return result;

            // Versuche Datei zu laden
            foreach (var resourcePack in resourcePacks)
            {
                string localFolder = Path.Combine(resourcePack.Path, basefolder);

                foreach (var textureType in textureTypes)
                {
                    string filename = Path.Combine(localFolder, string.Format("{0}.{1}", key, textureType));
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
                var resKey = fullkey.Replace(assemblyName, string.Format("{0}.Assets", assemblyName));
                foreach (var texturetype in textureTypes)
                {
                    using (var stream = baseType.Assembly.GetManifestResourceStream(string.Format("{0}.{1}", resKey, texturetype)))
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
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OctoAwesome.Client.Assets.OctoAwesome.Client.FallbackTexture.png"))
                {
                    result = callback(stream);
                }
            }

            // In Cache speichern
            if (result != null)
                cache[fullkey] = result;

            return result;
        }
    }
}
