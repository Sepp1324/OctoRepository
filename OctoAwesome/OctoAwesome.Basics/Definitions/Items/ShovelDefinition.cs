using System;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class ShovelDefinition : IItemDefinition
    {
        public string Name { get; }
        
        public string Icon { get; }

        public ShovelDefinition()
        {
            Name = "Shovel";
            Icon = "shovel_iron";
        }
        
        public bool CanMineMaterial(IMaterialDefinition material)=> material is ISolidMaterialDefinition solid;

        public Item Create(IMaterialDefinition material) => new Shovel(this, material);
    }
}
