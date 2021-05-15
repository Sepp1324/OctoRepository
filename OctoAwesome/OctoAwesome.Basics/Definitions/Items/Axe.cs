using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.MathLib;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class Axe : Item
    {
        private static readonly Polynomial _polynomial = new Polynomial(0, 3f/8f, 1f/800f, -1f/320000f);

        public Axe(AxeDefinition definition, IMaterialDefinition materialDefinition) : base(definition, materialDefinition)
        {

        }

        public override int Hit(IMaterialDefinition material, decimal blockVolumeVolumeRemaining, int volumePerHit)
        {
            var baseEfficiency =  base.Hit(material, blockVolumeVolumeRemaining, volumePerHit);

            if (material is ISolidMaterialDefinition solid && baseEfficiency > 0)
            {
                var fractureEfficiency = _polynomial.Evaluate(solid.FractureToughness);
                return (int) (baseEfficiency * (fractureEfficiency) / 100);
            }

            return baseEfficiency;
        }
    }
}
