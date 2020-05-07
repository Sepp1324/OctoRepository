using System;

namespace OctoAwesome
{
    /// <summary>
    /// Interface für das Persistieren eines Chunks
    /// </summary>
    public interface IPersistenceManager
    {
        void SaveUniverse(IUniverse universe);

        void SavePlanet(Guid universeGuid, IPlanet planet);

        void SaveColumn(Guid universeGuid, int planetId, IChunkColumn column);

        IUniverse[] ListUniverses();

        IUniverse LoadUniverse(Guid universeGuid);

        IPlanet LoadPlanet(Guid universeGuid, int planetId);

        IChunkColumn LoadColumn(Guid universeGuid, int planetId, Index2 columnIndex);
    }
}
