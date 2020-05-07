using System;
using System.IO;

namespace OctoAwesome
{
    public interface IUniverseSerializer
    {
        void Serialize(Stream stream, IUniverse universe);

        IUniverse Deserialize(Stream stream);
    }
}
