﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctoAwesome.Definitions;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Threading;

namespace OctoAwesome
{
    /// <summary>
    ///     Welt-Modell einer Säule aus <see cref="IChunk" />s.
    /// </summary>
    public class ChunkColumn : IChunkColumn
    {
        private static ChunkPool _chunkPool;

        /// <summary>
        ///     Auflistung aller sich in dieser Column befindenden Entitäten.
        /// </summary>
        private readonly IEntityList _entities;

        private readonly LockSemaphore _entitySemaphore;
        private readonly IGlobalChunkCache _globalChunkCache;

        /// <summary>
        ///     Erzeugt eine neue Instanz einer ChunkColumn.
        /// </summary>
        /// <param name="chunks">Die Chunks für die Säule</param>
        /// <param name="planet">Der Index des Planeten</param>
        /// <param name="columnIndex">Die Position der Säule</param>
        public ChunkColumn(IChunk[] chunks, IPlanet planet, Index2 columnIndex) : this(planet)
        {
            Chunks = chunks;
            Index = columnIndex;
            foreach (var chunk in chunks)
            {
                chunk.Changed += OnChunkChanged;
                chunk.SetColumn(this);
            }
        }

        /// <summary>
        ///     Erzeugt eine neue Instanz einer ChunkColumn.
        /// </summary>
        public ChunkColumn(IPlanet planet)
        {
            Heights = new int[Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y];
            _entities = new EntityList(this);
            _entitySemaphore = new(1, 1);
            DefinitionManager = TypeContainer.Get<IDefinitionManager>();
            Planet = planet;
            _globalChunkCache = planet.GlobalChunkCache;

            _chunkPool ??= TypeContainer.Get<ChunkPool>();
        }

        /// <summary>
        /// </summary>
        public IDefinitionManager DefinitionManager { get; }

        public int ChangeCounter { get; set; }

        /// <summary>
        ///     Höhen innerhalb der Chunk-Säule (oberste Blöcke)
        /// </summary>
        public int[,] Heights { get; }

        /// <summary>
        ///     Die Chunks der Säule.
        /// </summary>
        public IChunk[] Chunks { get; private set; }

        /// <summary>
        ///     Gibt an, ob die ChunkColumn schon von einem <see cref="IMapPopulator" /> bearbeitet wurde.
        /// </summary>
        public bool Populated { get; set; }

        /// <summary>
        ///     Der Index des Planeten.
        /// </summary>
        public IPlanet Planet { get; private set; }

        /// <summary>
        ///     Die Position der Säule.
        /// </summary>
        public Index2 Index { get; private set; }

        /// <summary>
        ///     Liefet den Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="index">Koordinate des Blocks innerhalb des Chunkgs</param>
        /// <returns>Die Block-ID an der angegebenen Koordinate</returns>
        public ushort GetBlock(Index3 index) => GetBlock(index.X, index.Y, index.Z);

        /// <summary>
        ///     Liefet den Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Block-ID der angegebenen Koordinate</returns>
        public ushort GetBlock(int x, int y, int z)
        {
            var index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlock(x, y, z);
        }

        /// <summary>
        ///     Gibt die Metadaten des Blocks an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Die Metadaten des angegebenen Blocks</returns>
        public int GetBlockMeta(int x, int y, int z)
        {
            var index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlockMeta(x, y, z);
        }

        /// <summary>
        ///     Liefert alle Ressourcen im Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Ein Array aller Ressourcen des Blocks</returns>
        public ushort[] GetBlockResources(int x, int y, int z)
        {
            var index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlockResources(x, y, z);
        }

        /// <summary>
        ///     Überschreibt den Block an der angegebenen Position.
        /// </summary>
        /// <param name="index">Koordinate des Zielblocks innerhalb des Chunks.</param>
        /// <param name="block">Neuer Block oder null, falls der vorhandene Block gelöscht werden soll</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        public void SetBlock(Index3 index, ushort block, int meta = 0)
        {
            SetBlock(index.X, index.Y, index.Z, block, meta);
        }

        /// <summary>
        ///     Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        /// <param name="block">Die neue Block-ID</param>
        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            var index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlock(x, y, z, block, meta);
        }

        public void SetBlocks(bool issueNotification, params BlockInfo[] blockInfos)
        {
            foreach (var item in blockInfos.GroupBy(x => x.Position.Z / Chunk.CHUNKSIZE_Z))
                Chunks[item.Key].SetBlocks(issueNotification, item.ToArray());
        }

        /// <summary>
        ///     Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            var index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlockMeta(x, y, z, meta);
        }

        /// <summary>
        ///     Ändert die Ressourcen des Blocks an der angegebenen Koordinate
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="resources">Ein <see cref="ushort" />-Array, das alle Ressourcen enthält</param>
        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            var index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlockResources(x, y, z, resources);
        }

        /// <summary>
        /// Serializes the ChunkColumn with the given <see cref="BinaryWriter"/>
        /// </summary>
        /// <param name="writer">Given <see cref="BinaryWriter"/></param>
        public void Serialize(BinaryWriter writer)
        {
            // Definitionen sammeln
            var definitions = new List<IBlockDefinition>();
            for (var c = 0; c < Chunks.Length; c++)
            {
                var chunk = Chunks[c];
                for (var i = 0; i < chunk.Blocks.Length; i++)
                    if (chunk.Blocks[i] != 0)
                    {
                        var definition = DefinitionManager.GetBlockDefinitionByIndex(chunk.Blocks[i]);
                        if (!definitions.Contains(definition))
                            definitions.Add(definition);
                    }
            }

            var longIndex = definitions.Count > 254;
            writer.Write((byte)(longIndex ? 1 : 0));

            // Schreibe Phase 1 (Column Meta: Heightmap, populated, chunkcount)
            writer.Write((byte)Chunks.Length); // Chunk Count
            writer.Write(Populated); // Populated
            writer.Write(Index.X);
            writer.Write(Index.Y);
            writer.Write(Planet.Id);

            for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++) // Heightmap
            for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
                writer.Write((ushort)Heights[x, y]);

            // Schreibe Phase 2 (Block Definitionen)
            if (longIndex)
                writer.Write((ushort)definitions.Count);
            else
                writer.Write((byte)definitions.Count);

            foreach (var definition in definitions)
                writer.Write(definition.GetType().FullName!);

            // Schreibe Phase 3 (Chunk Infos)
            for (var c = 0; c < Chunks.Length; c++)
            {
                var chunk = Chunks[c];
                writer.Write(chunk.Version);
                for (var i = 0; i < chunk.Blocks.Length; i++)
                    if (chunk.Blocks[i] == 0)
                    {
                        // Definition Index (Air)
                        if (longIndex)
                            writer.Write((ushort)0);
                        else
                            writer.Write((byte)0);
                    }
                    else
                    {
                        // Definition Index
                        var definition = DefinitionManager.GetBlockDefinitionByIndex(chunk.Blocks[i]);

                        if (longIndex)
                            writer.Write((ushort)(definitions.IndexOf(definition) + 1));
                        else
                            writer.Write((byte)(definitions.IndexOf(definition) + 1));

                        // Meta Data
                        if (definition.HasMetaData)
                            writer.Write(chunk.MetaData[i]);
                    }
            }

            var resManager = TypeContainer.Get<IResourceManager>();
            using var lockObj = _entitySemaphore.Wait();
            foreach (var entity in _entities)
                resManager.SaveEntity(entity);
        }

        /// <summary>
        /// Deserializes the ChunkColumn with the given <see cref="BinaryReader"/>
        /// </summary>
        /// <param name="reader">Given <see cref="BinaryReader"/></param>
        public void Deserialize(BinaryReader reader)
        {
            var longIndex = reader.ReadByte() > 0;

            // Phase 1 (Column Meta: Heightmap, populated, chunkcount)
            Chunks = new Chunk[reader.ReadByte()]; // Chunk Count

            Populated = reader.ReadBoolean(); // Populated

            Index = new(reader.ReadInt32(), reader.ReadInt32());
            var planetId = reader.ReadInt32();

            var resManager = TypeContainer.Get<IResourceManager>();
            Planet = resManager.GetPlanet(planetId);

            for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++) // Heightmap
            for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
                Heights[x, y] = reader.ReadUInt16();

            // Phase 2 (Block Definitionen)
            int typeCount = longIndex ? reader.ReadUInt16() : reader.ReadByte();
            var types = new List<IDefinition>();
            Span<ushort> map = stackalloc ushort[typeCount];

            for (var i = 0; i < typeCount; i++)
            {
                var typeName = reader.ReadString();
                var definitions = DefinitionManager.Definitions.ToArray();
                var blockDefinition = definitions.FirstOrDefault(d => d.GetType().FullName == typeName);
                types.Add(blockDefinition);

                map[types.Count - 1] = (ushort)(Array.IndexOf(definitions, blockDefinition) + 1);
            }

            // Phase 3 (Chunk Infos)
            for (var c = 0; c < Chunks.Length; c++)
            {
                var chunk = Chunks[c] = _chunkPool.Get(new(Index, c), Planet);
                chunk.Version = reader.ReadInt32();
                chunk.Changed += OnChunkChanged;
                chunk.SetColumn(this);

                for (var i = 0; i < chunk.Blocks.Length; i++)
                {
                    var typeIndex = longIndex ? reader.ReadUInt16() : reader.ReadByte();
                    chunk.MetaData[i] = 0;
                    if (typeIndex > 0)
                    {
                        var definitionIndex = map[typeIndex - 1];

                        chunk.Blocks[i] = definitionIndex;

                        var definition = DefinitionManager.GetBlockDefinitionByIndex(definitionIndex);

                        if (definition.HasMetaData)
                            chunk.MetaData[i] = reader.ReadInt32();
                    }
                }
            }
        }

        public event Action<IChunkColumn, IChunk> Changed;

        public void OnUpdate(SerializableNotification notification)
        {
            _globalChunkCache.OnUpdate(notification);
        }

        public void Update(SerializableNotification notification)
        {
            if (notification is IChunkNotification chunkNotification)
                Chunks
                    .FirstOrDefault(c => c.Index == chunkNotification.ChunkPos)?
                    .Update(notification);
        }

        public void ForEachEntity(Action<Entity> action)
        {
            using (_entitySemaphore.Wait())
            {
                foreach (var entity in _entities) action(entity);
            }
        }

        public void Add(Entity entity)
        {
            using (_entitySemaphore.Wait())
            {
                _entities.Add(entity);
            }
        }

        public void Remove(Entity entity)
        {
            using (_entitySemaphore.Wait())
            {
                _entities.Remove(entity);
            }
        }

        public IEnumerable<FailEntityChunkArgs> FailChunkEntity()
        {
            using (_entitySemaphore.Wait())
            {
                return _entities.FailChunkEntity().ToList();
            }
        }

        public void FlagDirty()
        {
            if (Chunks is null)
                return;

            foreach (var chunk in Chunks) chunk.FlagDirty();
        }

        private void OnChunkChanged(IChunk arg1)
        {
            ChangeCounter++;
            Changed?.Invoke(this, arg1);
        }

        /// <summary>
        ///     Errechnet die obersten Blöcke der Säule.
        /// </summary>
        public void CalculateHeights()
        {
            for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
            for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                Heights[x, y] = GetTopBlockHeight(x, y);
        }

        private int GetTopBlockHeight(int x, int y)
        {
            for (var z = Chunks.Length * Chunk.CHUNKSIZE_Z - 1; z >= 0; z--)
                if (GetBlock(x, y, z) != 0)
                    return z;
            return -1;
        }
    }
}