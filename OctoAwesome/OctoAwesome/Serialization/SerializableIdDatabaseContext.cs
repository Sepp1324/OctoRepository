using OctoAwesome.Database;

namespace OctoAwesome.Serialization
{
    public sealed class SerializableIdDatabaseContext<TObject> : SerializableDatabaseContext<IdTag, TObject> where TObject : ISerializable, IIdentification, new()
    {
        public SerializableIdDatabaseContext(Database<IdTag> database) : base(database)
        {

        }

        public override void AddOrUpdate(TObject value) => InternalAddOrUpdate(new IdTag(value.Id), value);

        public TObject Get(int key) => Get(new IdTag(key));

        public override void Remove(TObject value) => InternalRemove(new IdTag(value.Id));
    }
}
