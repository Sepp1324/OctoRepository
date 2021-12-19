using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.PoC
{
    public class DependencyTree
    {
        private readonly Dictionary<Type, DependencyLeaf> _leaves;

        public DependencyTree(Dictionary<Type, DependencyLeaf> leaves) => _leaves = leaves;

        public bool IsValid()
        {
            var leaves = _leaves.Values.ToList();

            foreach (var leaf in leaves)
            {
                if (leaf.Children.Any(child => child.Position <= leaf.Position))
                    return false;

                if (leaf.Parents.Any(parent => parent.Position >= leaf.Position))
                    return false;
            }

            return true;
        }
    }
}