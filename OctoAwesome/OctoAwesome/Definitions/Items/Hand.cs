namespace OctoAwesome.Definitions.Items
{
    public class Hand : Item
    {
        public Hand(HandDefinition handDefinition) : base(handDefinition, null)
        {
        }

        public override int Hit(IMaterialDefinition material, decimal volumeRemaining, int volumePerHit)
        {
            switch (material)
            {
                case ISolidMaterialDefinition {Granularity: > 1}:
                    return volumePerHit / 3;
                case IGasMaterialDefinition:
                case IFluidMaterialDefinition:
                    return 0;
                default:
                    return volumePerHit - material.Hardness / 2;
            }
        }
    }
}