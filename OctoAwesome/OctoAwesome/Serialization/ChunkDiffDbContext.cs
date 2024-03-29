﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctoAwesome.Database;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;

namespace OctoAwesome.Serialization
{
    public sealed class ChunkDiffDbContext : DatabaseContext<ChunkDiffTag, BlockChangedNotification>
    {
        private readonly IPool<BlockChangedNotification> _notificationBlockPool;

        public ChunkDiffDbContext(Database<ChunkDiffTag> database, IPool<BlockChangedNotification> blockPool) :
            base(database) =>
            _notificationBlockPool = blockPool;

        public override void AddOrUpdate(BlockChangedNotification value)
        {
            using (Database.Lock(Operation.Write))
            {
                InternalAddOrUpdate(new(value.ChunkPos, Chunk.GetFlatIndex(value.BlockInfo.Position)), value.BlockInfo);
            }
        }

        public void AddOrUpdate(BlocksChangedNotification value)
        {
            using (Database.Lock(Operation.Write))
            {
                value.BlockInfos.ForEach(b =>
                    InternalAddOrUpdate(new(value.ChunkPos, Chunk.GetFlatIndex(b.Position)), b));
            }
        }

        public IReadOnlyList<ChunkDiffTag> GetAllKeys() => Database.Keys;

        public override void Remove(BlockChangedNotification value)
        {
            InternalRemove(new(value.ChunkPos, Chunk.GetFlatIndex(value.BlockInfo.Position)));
        }

        public void Remove(BlocksChangedNotification value)
        {
            value.BlockInfos.ForEach(b => InternalRemove(new(value.ChunkPos, Chunk.GetFlatIndex(b.Position))));
        }

        public void Remove(params ChunkDiffTag[] tags)
        {
            foreach (var tag in tags)
                InternalRemove(tag);
        }

        public void Remove(IReadOnlyList<ChunkDiffTag> tags)
        {
            foreach (var tag in tags)
                InternalRemove(tag);
        }

        private void InternalRemove(ChunkDiffTag tag)
        {
            using (Database.Lock(Operation.Write))
            {
                Database.Remove(tag);
            }
        }

        private void InternalAddOrUpdate(ChunkDiffTag tag, BlockInfo blockInfo)
        {
            using var memory = new MemoryStream();
            using var writer = new BinaryWriter(memory);
            BlockInfo.Serialize(writer, blockInfo);
            Database.AddOrUpdate(tag, new(memory.ToArray()));
        }

        private BlockInfo InternalGet(ChunkDiffTag tag)
        {
            var value = Database.GetValue(tag);
            using var memory = new MemoryStream(value.Content);
            using var reader = new BinaryReader(memory);
            return BlockInfo.Deserialize(reader);
        }

        public override BlockChangedNotification Get(ChunkDiffTag key)
        {
            var notification = _notificationBlockPool.Get();
            notification.BlockInfo = InternalGet(key);
            notification.ChunkPos = key.ChunkPositon;
            return notification;
        }
    }
}