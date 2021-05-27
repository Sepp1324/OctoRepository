using OctoAwesome.Database;

namespace OctoAwesome.Serialization
{
<<<<<<< HEAD
    public abstract class SerializableDatabaseContext<TTag, TObject> : DatabaseContext<TTag, TObject> where TTag : ITag, new() where TObject : ISerializable, new()
=======
    public abstract class SerializableDatabaseContext<TTag, TObject> : DatabaseContext<TTag, TObject>
         where TTag : ITag, new()
         where TObject : ISerializable, new()
>>>>>>> feature/performance
    {
        protected SerializableDatabaseContext(Database<TTag> database) : base(database) { }

<<<<<<< HEAD
        public override TObject Get(TTag key) => Serializer.Deserialize<TObject>(Database.GetValue(key).Content);
=======
        public override TObject Get(TTag key)
            => Serializer.Deserialize<TObject>(Database.GetValue(key).Content);
>>>>>>> feature/performance

        protected void InternalRemove(TTag tag)
        {
            using (Database.Lock(Operation.Write))
                Database.Remove(tag);
        }

        protected void InternalAddOrUpdate(TTag tag, TObject value)
        {
            using (Database.Lock(Operation.Write))
                Database.AddOrUpdate(tag, new Value(Serializer.Serialize(value)));
        }
    }
}
