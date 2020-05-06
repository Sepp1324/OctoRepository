using System;
using System.IO;

namespace OctoAwesome
{
    public interface IPlanetSerializer
    {
        void Serialize(Stream stream, Guid universeId, IPlanet planet);

        IPlanet Deserialize(Stream stream, Guid universeId, int planetId);
    }
}
