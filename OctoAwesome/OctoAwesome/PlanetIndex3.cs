namespace OctoAwesome
{
    /// <summary>
    /// Datenstruktur zur genauen bestimmung eines Chunks und seinen Planeten
    /// </summary>
    public struct PlanetIndex3
    {
        /// <summary>
        /// Die Planeten-ID
        /// </summary>
        public readonly int PLANET;

        /// <summary>
        /// Die Position des Chunks
        /// </summary>
        public readonly Index3 CHUNK_INDEX;

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse PlanetIndex3
        /// </summary>
        /// <param name="planet">Der Index des Planeten</param>
        /// <param name="chunkIndex">Der <see cref="Index3"/> des Chunks</param>
        public PlanetIndex3(int planet, Index3 chunkIndex)
        {
            PLANET = planet;
            CHUNK_INDEX = chunkIndex;
        }

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse PlanetIndex3
        /// </summary>
        /// <param name="planet">Der Index des Planeten</param>
        /// <param name="x">X-Anteil des Indexes des Chunks</param>
        /// <param name="y">Y-Anteil des Indexes des Chunks</param>
        /// <param name="z">Z-Anteil des Indexes des Chunks</param>
        public PlanetIndex3(int planet, int x, int y, int z) : this(planet, new Index3(x, y, z)) { }

        /// <summary>
        /// Überprüft, ob beide gegebenen PlanetIndex3 den gleichen Wert aufweisen.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator ==(PlanetIndex3 i1, PlanetIndex3 i2) => i1.Equals(i2);

        /// <summary>
        /// Überprüft, ob beide gegebenen PlanetIndex3 nicht den gleichen Wert aufweisen.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator !=(PlanetIndex3 i1, PlanetIndex3 i2) => !i1.Equals(i2);

        /// <summary>
        /// Überprüft, ob der gegebene PlanetIndex3 den gleichen Wert aufweist, wie das gegebene Objekt.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is PlanetIndex3 other)
                return other.PLANET == PLANET && other.CHUNK_INDEX.X == CHUNK_INDEX.X && other.CHUNK_INDEX.Y == CHUNK_INDEX.Y && other.CHUNK_INDEX.Z == CHUNK_INDEX.Z;

            return false;
        }

        /// <summary>
        /// Erzeugt einen möglichst eindeutigen Hashcode des PlanetIndex3s
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => (PLANET << 24) + (CHUNK_INDEX.X << 16) + (CHUNK_INDEX.Y << 8) + CHUNK_INDEX.Z;
    }
}
