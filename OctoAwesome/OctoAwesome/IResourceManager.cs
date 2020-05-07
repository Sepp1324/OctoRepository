using System;

namespace OctoAwesome
{
    /// <summary>
    /// Interface, un die Ressourcen in OctoAwesome zu verfalten
    /// </summary>
    public interface IResourceManager
    {
        void LoadUniverse(Guid universeId);

        void UnloadUniverse();

        /// <summary>
        /// Gibt das Universum f�r die angegebene ID zur�ck
        /// </summary>
        /// <returns>Das gew�nschte Universum, falls es existiert</returns>
        IUniverse GetUniverse();
        
        /// <summary>
        /// Gibt den Planeten mit der angegebenen ID zur�ck
        /// </summary>
        /// <param name="planetId">Die Planteten-ID des gew�nschten Planeten</param>
        /// <returns>Der gew�nschte Planet, falls er existiert</returns>
        IPlanet GetPlanet(int planetId);

        /// <summary>
        /// Cache der f�r alle Chunks verwaltet und diese an lokale Caches weiter gibt.
        /// </summary>
        IGlobalChunkCache GlobalChunkCache { get; }
    }
}