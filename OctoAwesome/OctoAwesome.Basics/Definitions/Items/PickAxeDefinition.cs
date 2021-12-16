using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class PickaxeDefinition : IItemDefinition
    {
        public int StackLimit => 1;

        public float VolumePerUnit => 10;

        public string Icon => "pick_iron";

        public string Name => "Pickaxe";


        public bool CanMineMaterial(IMaterialDefinition material)
        {
            if (material is ISolidMaterialDefinition solid) return true;

            return false;
        }

        public Item Create(IMaterialDefinition material)
        {
            return new Pickaxe(this, material);
        }
    }
}