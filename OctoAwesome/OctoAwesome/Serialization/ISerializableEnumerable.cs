using System.Collections.Generic;

namespace OctoAwesome.Serialization
{
    public interface ISerializableEnumerable<T> : IEnumerable<T>, ISerializable where T : ISerializable
    {
    }
}