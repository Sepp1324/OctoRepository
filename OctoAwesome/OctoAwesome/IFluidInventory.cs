using OctoAwesome.Definitions;

namespace OctoAwesome
{
    /// <summary>
    /// Represents a Fluid Inventory (e.g. Bucket)
    /// </summary>
    public interface IFluidInventory
    {
        int MaxQuantity { get; }
        
        int Quantity { get; }
        
        IBlockDefinition Fluid { get; }
        
        /// <summary>
        /// Adds Fluid to the Fluid Inventory
        /// </summary>
        /// <param name="quantity">Amount of Fluid</param>
        /// <param name="fluid">Type of Fluid</param>
        void AddFluid(int quantity, IBlockDefinition fluid);
    }
}
