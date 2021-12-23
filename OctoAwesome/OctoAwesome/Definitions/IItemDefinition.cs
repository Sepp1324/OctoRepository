using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Definitions
{
    /// <summary>
    ///     Interface für die Definition eînes Items
    /// </summary>
    public interface IItemDefinition : IDefinition
    {
        /// <summary>
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        bool CanMineMaterial(IMaterialDefinition material);

        /// <summary>
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        Item Create(IMaterialDefinition material);
    }
}