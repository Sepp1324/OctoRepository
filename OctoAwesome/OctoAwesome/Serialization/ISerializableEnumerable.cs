using System.Collections.Generic;

namespace OctoAwesome.Serialization
{
    public interface ISerializableEnumerator<T> : IEnumerable<T>, ISerializable where T : ISerializable
    {
    }
}
