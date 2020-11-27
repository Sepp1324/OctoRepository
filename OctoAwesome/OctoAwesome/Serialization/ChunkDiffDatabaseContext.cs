using OctoAwesome.Database;
using OctoAwesome.Notifications;
using System.Collections.Generic;

namespace OctoAwesome.Serialization
{
    public sealed class ChunkDiffDatabaseContext : SerializableDatabaseContext<ChunkDiffTag, ChunkNotification>
    {
        public ChunkDiffDatabaseContext(Database<ChunkDiffTag> database) : base(database)
        {
        }

        public override void AddOrUpdate(ChunkNotification value)
            => InternalAddOrUpdate(new ChunkDiffTag(value.ChunkPos, value.FlatIndex), value);

        public IEnumerable<ChunkDiffTag> GetAllKeys() => Database.Keys;

        public override void Remove(ChunkNotification value)
            => InternalRemove(new ChunkDiffTag(value.ChunkPos, value.FlatIndex));

        public void Remove(params ChunkDiffTag[] tags)
        {
            foreach (var tag in tags)
                InternalRemove(tag);
        }
    }
}