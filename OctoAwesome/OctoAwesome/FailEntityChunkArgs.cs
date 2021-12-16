namespace OctoAwesome
{
    /// <summary>
    /// 
    /// </summary>
    public class FailEntityChunkArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Index2 CurrentChunk { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IPlanet CurrentPlanet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Index2 TargetChunk { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IPlanet TargetPlanet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Entity Entity { get; set; }
    }
}