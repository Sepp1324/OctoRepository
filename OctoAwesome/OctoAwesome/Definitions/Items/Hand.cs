namespace OctoAwesome.Definitions.Items
{
    /// <summary>
    /// 
    /// </summary>
    public class Hand : Item
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handDefinition"></param>
        public Hand(HandDefinition handDefinition) : base(handDefinition, null) { }

        public override int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining,
            int volumePerHit)
        {
            return material switch
            {
                ISolidMaterialDefinition { Granularity: > 1 } => volumePerHit / 3,
                IGasMaterialDefinition or IFluidMaterialDefinition => 0,
                _ => volumePerHit - material.Hardness / 2
            };
        }
    }
}