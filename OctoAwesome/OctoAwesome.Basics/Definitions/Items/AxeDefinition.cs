using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class AxeDefinition : IItemDefinition
    {
        public string Name { get; }
        
        public string Icon { get; }

        public AxeDefinition()
        {
            Name = "Axe";
            Icon = "axe_iron";
        }
        
        public bool CanMineMaterial(IMaterialDefinition material) => material is ISolidMaterialDefinition solid;

        public Item Create(IMaterialDefinition material) => new Axe(this, material);
    }
}
