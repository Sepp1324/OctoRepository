using OctoAwesome.Serialization;

namespace OctoAwesome.Components
{
    /// <summary>
    /// </summary>
    public interface IComponent : ISerializable
    {
        bool Sendable { get; set; }
        bool Enabled { get; set; }
    }
}