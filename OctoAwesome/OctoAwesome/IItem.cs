using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IItem
    {
        IEnumerable<IResource> Resources { get; }

        Coordinate? Position { get; }

        void Hit(IItem item);
    }
}
