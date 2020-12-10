using OctoAwesome.Database;
using OctoAwesome.Notifications;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Serialization
{
    public sealed class ChunkDiffDbContext : DatabaseContext<ChunkDiffTag, BlockChangedNotification>
    {
        public ChunkDiffDbContext(Database<ChunkDiffTag> database) : base(database)
        {
        }

        public override void AddOrUpdate(BlockChangedNotification value) => InternalAddOrUpdate(new ChunkDiffTag(value.ChunkPos, value.FlatIndex), value);

        public void AddOrUpdate(BlocksChangedNotification value) => value.BlockInfos.ForEach(b => InternalAddOrUpdate(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(b.Position)), b));

        public IEnumerable<ChunkDiffTag> GetAllKeys() => Database.Keys;

        public override void Remove(BlockChangedNotification value) => InternalRemove(new ChunkDiffTag(value.ChunkPos, value.FlatIndex));

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
                Serialize(binaryWriter, blockInfo);
                Database.AddOrUpdate(chunkDiffTag, new Value(memoryStream.ToArray()));
            }
        }

        public override BlockChangedNotification Get(ChunkDiffTag key)
        {
            throw new System.NotImplementedException();
        }

        private BlockInfo InternalGet(ChunkDiffTag chunkDiffTag)
        {
            var value = Database.GetValue(chunkDiffTag);

            using (var memoryStream = new MemoryStream(value.Content))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                return Deserialize(binaryReader);
            }
        }

        private static void Serialize(BinaryWriter binaryWriter, BlockInfo blockInfo)
        {
            binaryWriter.Write(blockInfo.Position.X);
            binaryWriter.Write(blockInfo.Position.Y);
            binaryWriter.Write(blockInfo.Position.Z);
            binaryWriter.Write(blockInfo.Block);
            binaryWriter.Write(blockInfo.Meta);
        }

        private static BlockInfo Deserialize(BinaryReader binaryReader) => new BlockInfo(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadUInt16(), binaryReader.ReadInt32());
    }
}
