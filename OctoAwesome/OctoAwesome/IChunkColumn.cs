using System;
using System.Collections.Generic;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Base-Interface for a ChunkColumn
    /// </summary>
    public interface IChunkColumn : ISerializable
    {
        /// <summary>
        /// States if the <see cref="IChunkColumn"/> already got edited by a <see cref="IMapPopulator"/>
        /// </summary>
        bool Populated { get; set; }

        /// <summary>
        /// Current <see cref="Planet"/>
        /// </summary>
        IPlanet Planet { get; }

        /// <summary>
        /// Position of the <see cref="ChunkColumn"/>
        /// </summary>
        Index2 Index { get; }

        /// <summary>
        /// Heights within the CunkColumn
        /// </summary>
        int[,] Heights { get; }

        /// <summary>
        /// <see cref="Chunks"/> of the <see cref="ChunkColumn"/>
        /// </summary>
        IChunk[] Chunks { get; }

        /// <summary>
        /// Returns the Blocks of the given Coordinate
        /// </summary>
        /// <param name="index">Coordinate of the Block within the <see cref="ChunkColumn"/></param>
        /// <returns>Block-ID of the given Coordinate</returns>
        ushort GetBlock(Index3 index);

        /// <summary>
        /// Returns List of <see cref="Block"/> at the given Coordinate
        /// </summary>
        /// <param name="x">X-Axis of Block-Coordinate</param>
        /// <param name="y">Y-Axis of Block-Coordinate</param>
        /// <param name="z">Z-Axis of Block-Coordinate</param>
        /// <returns>Block-ID of the given Coordinate</returns>
        ushort GetBlock(int x, int y, int z);

        /// <summary>
        /// Action for ChunkColumn Changes
        /// </summary>
        event Action<IChunkColumn, IChunk> Changed;

        /// <summary>
        /// Overrides the Block of the given Position
        /// </summary>
        /// <param name="index">Koordinate des Zielblocks innerhalb des Chunks.</param>
        /// <param name="block">Neuer Block oder null, falls der vorhandene Block gelöscht werden soll</param>
        /// <param name="meta">(Optional) MetaInformation of the Block</param>
        void SetBlock(Index3 index, ushort block, int meta = 0);

        /// <summary>
        /// Overrides the Block of the given Position
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        /// <param name="block">Die neue Block-ID</param>
        void SetBlock(int x, int y, int z, ushort block, int meta = 0);

        /// <summary>
        ///     Gibt die Metadaten des Blocks an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Die Metadaten des angegebenen Blocks</returns>
        int GetBlockMeta(int x, int y, int z);

        /// <summary>
        ///     Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        void SetBlockMeta(int x, int y, int z, int meta);

        /// <summary>
        ///     Liefert alle Ressourcen im Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Ein Array aller Ressourcen des Blocks</returns>
        ushort[] GetBlockResources(int x, int y, int z);

        /// <summary>
        ///     Ändert die Ressourcen des Blocks an der angegebenen Koordinate
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="resources">Ein <see cref="ushort" />-Array, das alle Ressourcen enthält</param>
        void SetBlockResources(int x, int y, int z, ushort[] resources);

        /// <summary>
        /// Event for Block-Update within a ChunkColumn
        /// </summary>
        /// <param name="notification"></param>
        void OnUpdate(SerializableNotification notification);

        void Update(SerializableNotification notification);

        void ForEachEntity(Action<Entity> action);

        IEnumerable<FailEntityChunkArgs> FailChunkEntity();

        void Remove(Entity entity);

        void Add(Entity entity);

        void SetBlocks(bool issueNotification, params BlockInfo[] blockInfos);

        void FlagDirty();
    }
}