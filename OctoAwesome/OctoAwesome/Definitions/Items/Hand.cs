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
        public Hand(HandDefinition handDefinition) : base(handDefinition, null)
        {
        }

        public override int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining,
            int volumePerHit)
        {
            if (material is ISolidMaterialDefinition { Granularity: > 1 }) return volumePerHit / 3;
            if (material is IGasMaterialDefinition or IFluidMaterialDefinition)
                return 0;

            return volumePerHit - material.Hardness / 2;
        }
    }
}