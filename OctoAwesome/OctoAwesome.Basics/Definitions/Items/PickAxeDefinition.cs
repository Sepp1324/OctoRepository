using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class PickaxeDefinition : IItemDefinition
    {
        public string Icon => "pick_iron";

        public string Name => "Pickaxe";

        public int StackLimit => 1;

        public float VolumePerUnit => 10;
        
        public bool CanMineMaterial(IMaterialDefinition material) => material is ISolidMaterialDefinition solid;

        public Item Create(IMaterialDefinition material) => new Pickaxe(this, material);
    }
}
