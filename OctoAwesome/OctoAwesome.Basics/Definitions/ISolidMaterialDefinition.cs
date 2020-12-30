using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions
{
    public interface ISolidMaterialDefinition : IMaterialDefinition
    {
        int Granularity { get; }
    }
}