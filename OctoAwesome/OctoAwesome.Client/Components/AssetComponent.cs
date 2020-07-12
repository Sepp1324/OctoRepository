using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Client.Components
{
    internal sealed class AssetComponent : DrawableGameComponent
    {
        Dictionary<string, Texture2D> cache;

        string[] textureTypes = new string[] { "png", "jpg", "jpeg", "bmp" };

        List<string> resourcePacks = new List<string>();

        public AssetComponent(OctoGame game) : base(game)
        {
            cache = new Dictionary<string, Texture2D>();
        }

        public Texture2D LoadTexture(Type baseType, string key)
        {
            if (baseType == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException();

            string basefolder = baseType.Namespace.
                //Replace("OctoAwesome.", "").
                Replace('.', Path.DirectorySeparatorChar);

            //Cache fragen
            if (cache.ContainsKey(basefolder))
                return cache[basefolder];

            //Versuchen Datei zu laden
            Texture2D result = null;
            foreach (var resourcePack in resourcePacks)
            {
                string localFolder = Path.Combine(resourcePack, basefolder);

                foreach (var textureType in textureTypes)
                {
                    string filename = Path.Combine(localFolder, string.Format("{0}.{1}", key, textureType));
                    if (File.Exists(filename))
                    {
                        using (var stream = File.Open(filename, FileMode.Open))
                        {
                            result = Texture2D.FromStream(GraphicsDevice, stream);
                        }
                        break;
                    }
                }

                if (result != null)
                    break;
            }

            //Resourcefallback
            if(result == null)
            {

            }

            if(result == null)
            {
                //TODO: WorstCase: CheckerTex laden
            }

            //In Cache speichern
            if (result != null)
                cache[basefolder] = result;

            return result;
        }
    }
}
