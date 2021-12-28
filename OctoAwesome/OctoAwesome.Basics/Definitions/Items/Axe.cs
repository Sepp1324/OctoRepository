using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.OctoMath;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class Axe : Item
    {
        private static readonly Polynomial Polynomial;

        static Axe() => Polynomial = new(0, 3f / 8f, 1f / 800f, -1f / 320000f);

        public Axe() : base(null, null) { }

        public Axe(AxeDefinition definition, IMaterialDefinition materialDefinition) : base(definition, materialDefinition) { }

        public override int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining, int volumePerHit)
        {
            var baseEfficiency = base.Hit(material, blockInfo, volumeRemaining, volumePerHit);

            if (material is not ISolidMaterialDefinition solid || baseEfficiency <= 0) 
                return baseEfficiency;

            var fractureEfficiency = Polynomial.Evaluate(solid.FractureToughness);
            return (int)(baseEfficiency * fractureEfficiency / 100);

        }
    }
}