using OctoAwesome.Components;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LocalChunkCacheComponent : Component, IEntityComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public LocalChunkCacheComponent() { }

        /// <summary>
        /// 
        /// </summary>
        public LocalChunkCacheComponent(IGlobalChunkCache globalChunkCache, int dimensions, int range) => LocalChunkCache = new LocalChunkCache(globalChunkCache, dimensions, range);

        /// <summary>
        /// 
        /// </summary>
        public ILocalChunkCache LocalChunkCache { get; set; }
    }
}