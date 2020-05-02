using System.Drawing;

namespace OctoAwesome
{
    public interface IItemDefinition
    {
        string Name { get; }

        Bitmap Icon { get; }

        PhysicalProperties GetProperties(IItem item);

        void Hit(IItem item, PhysicalProperties itemProperties);
    }
}