using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.PoC
{
    public class DependencyTree
    {
        private readonly List<DependencyLeaf> _leaves;

        public DependencyTree(List<DependencyLeaf> leaves) => _leaves = leaves;

        public bool IsValid()
        {
            foreach (var leaf in _leaves)
            {
                if (leaf.Children.Any(child => child.Position <= leaf.Position))
                    return false;

                if (leaf.Parents.Any(parent => parent.Position >= leaf.Position))
                    return false;
            }

            foreach (var leaf in _leaves)
            {
                if (leaf.Children.Any(child => _leaves.IndexOf(child) < _leaves.IndexOf(leaf)))
                    return false;

                if (leaf.Parents.Any(parent => _leaves.IndexOf(parent) > _leaves.IndexOf(leaf)))
                    return false;
            }

            return true;
        }
    }
}