namespace OctoAwesome.EntityComponents
{
    public sealed class LocalChunkCacheComponent : EntityComponent
    {
        public LocalChunkCacheComponent()
        {
        }

        public LocalChunkCacheComponent(IGlobalChunkCache globalChunkCache, int dimensions, int range) => LocalChunkCache = new LocalChunkCache(globalChunkCache, dimensions, range);

        public ILocalChunkCache LocalChunkCache { get; set; }
    }
}