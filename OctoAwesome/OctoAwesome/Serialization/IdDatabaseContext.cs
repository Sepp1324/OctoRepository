using System;
using OctoAwesome.Database;

namespace OctoAwesome.Serialization
{
    public sealed class IdDatabaseContext<TObject> : SerializableDatabaseContext<GuidTag<int>, TObject> where TObject : ISerializable, IIdentification, new()
    {
        public IdDatabaseContext(Database<GuidTag<int>> database) : base(database)
        {
        }

        public override void AddOrUpdate(TObject value)
        {
            InternalAddOrUpdate(new GuidTag<int>(value.Id), value);
        }

        public TObject Get(Guid key)
        {
            return Get(new GuidTag<int>(key));
        }

        public override void Remove(TObject value)
        {
            InternalRemove(new GuidTag<int>(value.Id));
        }
    }
}