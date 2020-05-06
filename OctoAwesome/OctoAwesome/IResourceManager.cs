namespace OctoAwesome
{
    /// <summary>
    /// Interface, un die Ressourcen in OctoAwesome zu verfalten
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// Gibt das Universum f�r die angegebene ID zur�ck
        /// </summary>
        /// <param name="id">Die ID des Universums</param>
        /// <returns>Das gew�nschte Universum, falls es existiert</returns>
        IUniverse GetUniverse(int id);
        
        /// <summary>
        /// Gibt den Planeten mit der angegebenen ID zur�ck
        /// </summary>
        /// <param name="id">Die Planteten-ID des gew�nschten Planeten</param>
        /// <returns>Der gew�nschte Planet, falls er existiert</returns>
        IPlanet GetPlanet(int id);

        /// <summary>
        /// Cache der f�r alle Chunks verwaltet und diese an lokale Caches weiter gibt.
        /// </summary>
        IGlobalChunkCache GlobalChunkCache { get; }
    }
}