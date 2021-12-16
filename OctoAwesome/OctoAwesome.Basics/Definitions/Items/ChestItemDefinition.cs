using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class ChestItemDefinition : IItemDefinition
    {
        public ChestItemDefinition()
        {
            Name = "Chest";
            Icon = "chest";
        }

        public string Name { get; }
        public string Icon { get; }

        public bool CanMineMaterial(IMaterialDefinition material)
        {
            return false;
        }

        public Item Create(IMaterialDefinition material)
        {
            return new ChestItem(this, material);
        }
    }
}