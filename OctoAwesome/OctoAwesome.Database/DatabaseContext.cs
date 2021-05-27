namespace OctoAwesome.Database
{
    public abstract class DatabaseContext<Tag, TObject> : IDatabaseContext<Tag, TObject> where Tag : ITag, new()
    {
        protected DatabaseContext(Database<Tag> database)
        {
            Database = database;
        }

        protected Database<Tag> Database { get; }

        public abstract TObject Get(Tag key);

        public abstract void AddOrUpdate(TObject value);

        public abstract void Remove(TObject value);
    }
}