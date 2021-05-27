namespace OctoAwesome.Database
{
    public abstract class DatabaseContext<Tag, TObject> : IDatabaseContext<Tag, TObject> where Tag : ITag, new()
    {
        protected Database<Tag> Database { get; }

<<<<<<< HEAD
        protected DatabaseContext(Database<Tag> database) => Database = database;
=======
        protected DatabaseContext(Database<Tag> database)
        {
            Database = database;
        }
>>>>>>> feature/performance

        public abstract TObject Get(Tag key);

        public abstract void AddOrUpdate(TObject value);

        public abstract void Remove(TObject value);
    }
}
