using System;
using System.IO;

namespace OctoAwesome.Runtime
{
    public sealed class PlanetSerializer : IPlanetSerializer
    {
        public IPlanet Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream stream, IPlanet planet)
        {
            throw new NotImplementedException();
        }
    }
}
