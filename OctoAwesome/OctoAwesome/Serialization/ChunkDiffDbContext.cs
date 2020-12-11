using OctoAwesome.Database;
using OctoAwesome.Notifications;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctoAwesome.Pooling;

namespace OctoAwesome.Serialization
{
    public sealed class ChunkDiffDbContext : DatabaseContext<ChunkDiffTag, BlockChangedNotification>
    {
        private readonly IPool<BlockChangedNotification> _notificationBlockPool;

        public ChunkDiffDbContext(Database<ChunkDiffTag> database, IPool<BlockChangedNotification> blockPool) : base(database) => _notificationBlockPool = blockPool;

        public override void AddOrUpdate(BlockChangedNotification value) => InternalAddOrUpdate(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(value.BlockInfo.Position)), value.BlockInfo);

        public void AddOrUpdate(BlocksChangedNotification value) => value.BlockInfos.ForEach(b => InternalAddOrUpdate(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(b.Position)), b));

        public IEnumerable<ChunkDiffTag> GetAllKeys() => Database.Keys;

        public override void Remove(BlockChangedNotification value) => InternalRemove(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(value.BlockInfo.Position)));

        public void Remove(BlocksChangedNotification value) => value.BlockInfos.ForEach(b => InternalRemove(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(b.Position))));

        public void Remove(params ChunkDiffTag[] tags)
        {
            foreach (var tag in tags)
                InternalRemove(tag);
        }

        private void InternalRemove(ChunkDiffTag chunkDiffTag) => Database.Remove(chunkDiffTag);

        private void InternalAddOrUpdate(ChunkDiffTag chunkDiffTag, BlockInfo blockInfo)
        {
            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                BlockInfo.Serialize(binaryWriter, blockInfo);
                Database.AddOrUpdate(chunkDiffTag, new Value(memoryStream.ToArray()));
            }
        }

        public override BlockChangedNotification Get(ChunkDiffTag key)
        {
            var notification = _notificationBlockPool.Get();
            notification.BlockInfo = InternalGet(key);
            notification.ChunkPos = key.ChunkPositon;
            return notification;
        }

        private BlockInfo InternalGet(ChunkDiffTag chunkDiffTag)
        {
            var value = Database.GetValue(chunkDiffTag);

            using (var memoryStream = new MemoryStream(value.Content))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                return BlockInfo.Deserialize(binaryReader);
            }
        }
    }
}
