﻿using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class DiskPersistenceManager : IPersistenceManager
    {
        private const string UniverseFilename = "universe.info";

        private const string PlanetGeneratorInfo = "generator.info";

        private const string PlanetFilename = "planet.info";

        private const string ColumnFilename = "column_{0}_{1}.dat";

        public DiskPersistenceManager()
        {
        }

        private DirectoryInfo root;

        private string GetRoot()
        {
            if (root != null)
                return root.FullName;

            string appconfig = ConfigurationManager.AppSettings["ChunkRoot"];
            if (!string.IsNullOrEmpty(appconfig))
            {
                root = new DirectoryInfo(appconfig);
                if (!root.Exists) root.Create();
                return root.FullName;
            }
            else
            {
                var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                root = new DirectoryInfo(exePath + Path.DirectorySeparatorChar + "OctoMap");
                if (!root.Exists) root.Create();
                return root.FullName;
            }
        }

        public void SaveUniverse(IUniverse universe)
        {
            string path = Path.Combine(GetRoot(), universe.Id.ToString());
            Directory.CreateDirectory(path);

            string file = Path.Combine(path, UniverseFilename);
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            {
                universe.Serialize(stream);
            }
        }

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
                planet.Serialize(stream);
            }
        }

        public void SaveColumn(Guid universeGuid, int planetId, IChunkColumn column)
        {
            string path = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString());
            Directory.CreateDirectory(path);

            string file = path = Path.Combine(path, string.Format(ColumnFilename, column.Index.X, column.Index.Y));
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            {
                column.Serialize(stream, DefinitionManager.Instance);
            }
        }

        public IUniverse[] ListUniverses()
        {
            string root = GetRoot();
            List<IUniverse> universes = new List<IUniverse>();
            foreach (var folder in Directory.GetDirectories(root))
            {
                string id = folder.Replace(root, "");
                Guid guid;
                if (Guid.TryParse(id, out guid))
                    universes.Add(LoadUniverse(guid));
            }

            return universes.ToArray();
        }

        public IUniverse LoadUniverse(Guid universeGuid)
        {
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), UniverseFilename);
            if (!File.Exists(file))
                return null;

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                IUniverse universe = new Universe();
                universe.Deserialize(stream);
                return universe;
            }
        }

        public IPlanet LoadPlanet(Guid universeGuid, int planetId)
        {
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString(), PlanetFilename);
            string generatorInfo = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString(), PlanetGeneratorInfo);
            if (!File.Exists(generatorInfo) || !File.Exists(file))
                return null;

            IMapGenerator generator = null;
            using (Stream stream = File.Open(generatorInfo, FileMode.Create, FileAccess.Read))
            {
                using (BinaryReader bw = new BinaryReader(stream))
                {
                    string generatorName = bw.ReadString();
                    generator = MapGeneratorManager.GetMapGenerators().FirstOrDefault(g => g.GetType().FullName.Equals(generatorName));
                }
            }

            if (generator == null)
                throw new Exception("Unknown Generator");


            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                return generator.GeneratePlanet(stream);
            }
        }

        public IChunkColumn LoadColumn(Guid universeGuid, IPlanet planet, Index2 columnIndex)
        {
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), planet.Id.ToString(), string.Format(ColumnFilename, columnIndex.X, columnIndex.Y));
            if (!File.Exists(file))
                return null;

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                return planet.Generator.GenerateColumn(stream, DefinitionManager.Instance, planet.Id, columnIndex);
            }
        }
    }
}
