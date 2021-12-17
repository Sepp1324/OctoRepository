using System.Collections.Generic;

namespace OctoAwesome.PoC
{
    public class DependencyLeaf
    {
        public DependencyLeaf Child { get; set; }

        public DependencyLeaf Parent { get; set; }

        public List<DependencyItem> Items { get; set; }
    }
}
