using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public abstract class Item : IItem
    {

        public Item()
        {
            Resources = new List<IResource>();
            Condition = 99;
        }

        public List<IResource> Resources { get; private set; }

        public Coordinate? Position { get; set; }

        public int Condition { get; set; }

        public abstract void Hit(IItem item);
    }
}
