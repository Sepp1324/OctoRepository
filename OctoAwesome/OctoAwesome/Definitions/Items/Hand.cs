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
                case ISolidMaterialDefinition solidMaterial when solidMaterial.Granularity > 1:
                    return volumePerHit / 3;
                case IGasMaterialDefinition _:
                case IFluidMaterialDefinition _:
                    return 0;
                default:
                    return volumePerHit - material.Hardness / 2;
            }
        }
    }
}
