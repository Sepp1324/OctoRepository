using System;
using System.IO;

namespace OctoAwesome
{
    public interface IPlanetSerializer
    {
        void Serialize(Stream stream, IPlanet planet);

        IPlanet Deserialize(Stream stream);
    }
}
