namespace OctoAwesome.EntityComponents
{
    public sealed class LocalChunkCacheComponent : EntityComponent
    {
        public ILocalChunkCache LocalChunkCache { get; set; }

<<<<<<< HEAD
        public LocalChunkCacheComponent() { }
        
        public LocalChunkCacheComponent(IGlobalChunkCache globalChunkCache, int dimensions,int range) => LocalChunkCache = new LocalChunkCache(globalChunkCache, dimensions, range);
=======
        public LocalChunkCacheComponent()
        {
        }
        public LocalChunkCacheComponent(IGlobalChunkCache globalChunkCache, int dimensions,int range)
        {
            LocalChunkCache = new LocalChunkCache(globalChunkCache, dimensions, range);
        }
>>>>>>> feature/performance
    }
}
