using OctoAwesome.Basics.Definitions.Items;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.OctoMath;

namespace OctoAwesome.Basics
{
    public class Pickaxe : Item
    {
        private static readonly Polynomial Polynomial;

        static Pickaxe() => Polynomial = new(150, 0, -1f / 400f);

        public Pickaxe(PickaxeDefinition pickaxeDefinition, IMaterialDefinition materialDefinition) : base(pickaxeDefinition, materialDefinition) { }

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