﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace OctoAwesome.Runtime
{
    public class ResourceManager : IResourceManager
    {
        public static int CacheSize = 10000;

        private Guid DEFAULT_UNIVERSE = Guid.Parse("{9EAC0932-113A-403D-8E36-404EC30114A0}");

        private bool disablePersistence = false;

        private IMapGenerator mapGenerator = null;
        private IPersistenceManager chunkPersistence = null;

        private GlobalChunkCache globalChunkCache = null;

        private List<IMapPopulator> populators = null;

        /// <summary>
        /// Planet Cache.
        /// </summary>
        // private Cache<int, IPlanet> planetCache;

        private IPlanet[] _planets;

        private IUniverse universeCache;

        #region Singleton

        private static ResourceManager instance = null;
        public static ResourceManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ResourceManager();
                return instance;
            }
        }

        #endregion

        private ResourceManager()
        {
            mapGenerator = MapGeneratorManager.GetMapGenerators().First();
            chunkPersistence = new DiskPersistenceManager(new UniverseSerializer(), new PlanetSerializer(), new ColumnSerializer());

            globalChunkCache = new GlobalChunkCache(
                (p, i) => loadChunkColumn(p, i),
                (p, i, c) => saveChunkColumn(p, i, c));

            _planets = new[] { loadPlanet(0) };

            //planetCache = new Cache<int, IPlanet>(1, loadPlanet, savePlanet);
            //chunkCache = new Cache<PlanetIndex3, IChunk>(CacheSize, loadChunk, saveChunk);

            bool.TryParse(ConfigurationManager.AppSettings["DisablePersistence"], out disablePersistence);
        }

        public IGlobalChunkCache GlobalChunkCache { get { return globalChunkCache; } }

        public IUniverse GetUniverse(Guid id)
        {
            if (universeCache == null)
                universeCache = mapGenerator.GenerateUniverse(id);

            return universeCache;
        }

        public IPlanet GetPlanet(int id)
        {
            return _planets[id];
        }

        private IPlanet loadPlanet(int index)
        {
            IUniverse universe = GetUniverse(DEFAULT_UNIVERSE);

            return mapGenerator.GeneratePlanet(universe.Id, 4567);
        }

        private void savePlanet(int index, IPlanet value)
        {
        }

        private IChunkColumn loadChunkColumn(int planetId, Index2 index)
        {
            IUniverse universe = GetUniverse(DEFAULT_UNIVERSE);
            IPlanet planet = GetPlanet(planetId);

            // Load from disk
            IChunk[] chunks = new IChunk[planet.Size.Z];
            bool full = true;
            IChunkColumn column = chunkPersistence.LoadColumn(universe.Id, planetId, index);

            //for (int z = 0; z < planet.Size.Z; z++)
            //{
            //    IChunk chunk = chunkPersistence.Load(universe.Id, planetId, new Index3(index, z));
            //    if (chunk == null)
            //        full = false;
            //    chunks[z] = chunk;
            //}

            if (column == null)
            {
                IChunk[] generated = mapGenerator.GenerateChunk(DefinitionManager.GetBlockDefinitions(), planet, new Index2(index.X, index.Y));
                for (int z = 0; z < planet.Size.Z; z++)
                {
                    if (chunks[z] == null)
                        chunks[z] = generated[z];
                }
            }

            IChunkColumn column00 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(-1, -1), planet.Size));
            IChunkColumn column10 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(0, -1), planet.Size));
            IChunkColumn column20 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(1, -1), planet.Size));

            IChunkColumn column01 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(-1, 0), planet.Size));
            IChunkColumn column21 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(1, 0), planet.Size));

            IChunkColumn column02 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(-1, 1), planet.Size));
            IChunkColumn column12 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(0, 1), planet.Size));
            IChunkColumn column22 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(1, 1), planet.Size));

            IChunkColumn column11 = new ChunkColumn(chunks, planet.Id, index);

            //Populatoren erzeugen
            if (populators == null)
                populators = ExtensionManager.GetInstances<IMapPopulator>().OrderBy(p => p.Order).ToList();

            var definitions = DefinitionManager.GetBlockDefinitions();

            // Zentrum
            if (!column11.Populated && column21 != null && column12 != null && column22 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(definitions, planet, column11, column21, column12, column22);
                column11.Populated = true;
            }

            // Links oben
            if (column00 != null && !column00.Populated && column10 != null && column01 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(definitions, planet, column00, column10, column01, column11);
                column00.Populated = true;
            }

            // Oben
            if (column10 != null && !column10.Populated && column20 != null && column21 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(definitions, planet, column10, column20, column11, column21);
                column10.Populated = true;
            }

            // Links
            if (column01 != null && !column01.Populated && column02 != null && column12 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(definitions, planet, column01, column11, column02, column12);
                column01.Populated = true;
            }

            return column11;
        }

        private void saveChunkColumn(int planetId, Index2 index, IChunkColumn value)
        {
            IUniverse universe = GetUniverse(DEFAULT_UNIVERSE);

            if (!disablePersistence && value.Chunks.Any(c => c.ChangeCounter > 0))
            {
                chunkPersistence.SaveColumn(universe.Id, planetId, value);

                foreach (var chunk in value.Chunks)
                    chunk.ChangeCounter = 0;
            }
        }
    }
}