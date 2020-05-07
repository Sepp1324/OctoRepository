using System;
using System.IO;

namespace OctoAwesome.Runtime
{
    public sealed class UniverseSerializer : IUniverseSerializer
    {
        public IUniverse Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream stream, IUniverse universe)
        {
            throw new NotImplementedException();
        }
    }
}
