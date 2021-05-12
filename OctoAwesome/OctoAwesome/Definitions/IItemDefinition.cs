namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Interface für die Definition eînes Items
    /// </summary>
    public interface IItemDefinition : IDefinition, IInventoryableDefinition
    {
        bool CanMineMaterial(IMaterialDefinition material);
    }
}
