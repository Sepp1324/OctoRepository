using OctoAwesome.Database;

namespace OctoAwesome.Serialization
{
    public sealed class SerializableDatabaseContext<TObject> : DatabaseContext<IdTag, int, TObject> where TObject : ISerializable, IIdentification, new()
    {
        public SerializableDatabaseContext(Database<IdTag> database) : base(database)
        {
        }

        public override TObject Get(int key) => Serializer.Deserialize<TObject>(Database.GetValue(new IdTag(key)).Content);

        public override void AddOrUpdate(TObject value) => Database.AddOrUpdate(new IdTag(value.Id), new Value(Serializer.Serialize(value)));
        

        public override void Remove(TObject value) => Database.Remove(new IdTag(value.Id));
    }
}