<<<<<<< HEAD
﻿using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.MathLib;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class Pickaxe : Item
    {
        private static readonly Polynomial _polynomial = new Polynomial(150, 0, -1f/400f);
        
        public Pickaxe(PickaxeDefinition pickaxeDefinition, IMaterialDefinition materialDefinition) : base(pickaxeDefinition, materialDefinition)
=======
﻿using OctoAwesome.Basics.Definitions.Items;

namespace OctoAwesome.Basics
{
    public class Pickaxe : Item
    {
        public Pickaxe(PickaxeDefinition pickaxeDefinition) : base(pickaxeDefinition)
        {

        }

        public override void Hit(IItem item)
>>>>>>> feature/performance
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
