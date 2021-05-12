using OctoAwesome.Information;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class PickaxeDefinition : IItemDefinition
    {
        public string Icon => "pick_iron";

        public string Name => "Pickaxe";

        public int StackLimit => 1;

        public float VolumePerUnit => 10;

        int IInventoryableDefinition.VolumePerUnit => 1;
        
        public bool CanMineMaterial(IMaterialDefinition material)
        {
            return material is ISolidMaterialDefinition solid;
        }
    }
}
