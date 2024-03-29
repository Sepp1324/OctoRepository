﻿using System;
using OctoAwesome.Notifications;

namespace OctoAwesome
{
    /// <summary>
    ///     Basisinterface für einen Globalen Chunkcache
    /// </summary>
    public interface IGlobalChunkCache
    {
        /// <summary>
        ///     Die Zahl der geladenen Chunks zurück
        /// </summary>
        int LoadedChunkColumns { get; }

        /// <summary>
        ///     Anzahl der noch nicht gespeicherten ChunkColumns.
        /// </summary>
        int DirtyChunkColumn { get; }

        IPlanet Planet { get; }
        event EventHandler<IChunkColumn> ChunkColumnChanged;

        /// <summary>
        ///     Abonniert einen Chunk.
        /// </summary>
        /// <param name="position">Position des Chunks</param>
        /// <returns>Den neu abonnierten Chunk</returns>
        IChunkColumn Subscribe(Index2 position);

        bool IsChunkLoaded(Index2 position);

        /// <summary>
        /// Returns the Chunk if loaded
        /// </summary>
        /// <param name="position">Position of the loaded Chunk</param>
        /// <returns>Chunk-Instance, null if not loaded</returns>
        IChunkColumn Peek(Index2 position);

        /// <summary>
        ///     Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        void Release(Index2 position);

        /// <summary>
        ///     Löscht den gesamten Inhalt des Caches.
        /// </summary>
        void Clear();

        void BeforeSimulationUpdate(Simulation simulation);

        void AfterSimulationUpdate(Simulation simulation);

        void OnUpdate(SerializableNotification notification);

        void Update(SerializableNotification notification);
    }
}