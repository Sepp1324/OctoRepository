using System.Collections.Generic;

namespace OctoAwesome.PoC
{
    public class DependencyTree
    {
        private readonly List<DependencyLeaf> _roots;

        public DependencyTree() => _roots = new();

        public void Add(DependencyLeaf dependencyLeaf)
        {

        }

        public DependencyLeaf Get(string name) => default;

        public void Remove(DependencyLeaf dependencyLeaf)
        {

        }
    }
}
