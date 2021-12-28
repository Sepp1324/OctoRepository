using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.PoC
{
    public class DependencyLeaf
    {
        private IReadOnlyList<DependencyLeaf> _flattenedParents;

        public DependencyItem Item { get; }

        public Type ItemType { get; }

        /// <summary>
        /// This loads before these items
        /// </summary>
        public List<DependencyLeaf> Children { get; }

        /// <summary>
        /// This loads after these items
        /// </summary>
        public List<DependencyLeaf> Parents { get; }

        public int Position { get; internal set; }

        public DependencyLeaf(DependencyItem item, List<DependencyLeaf> children, List<DependencyLeaf> parents, int position)
        {
            Item = item;
            ItemType = item.Type;
            Children = children;
            Parents = parents;
            Position = position;
        }

        private IReadOnlyList<DependencyLeaf> GenerateFlattenedParents() => throw new NotImplementedException();

        private static bool HasCycle(ICollection<DependencyLeaf> dependencies)
        {
            HashSet<DependencyLeaf> visitedChildren = new();

            foreach (var dependency in dependencies)
            {
                foreach (var child in dependency.Children)
                {
                    if (visitedChildren.Contains(child))
                        continue;

                    if (CheckCycleForChildren(child, dependency, dependencies.Count - 1, visitedChildren))
                        return true;

                    visitedChildren.Add(child);
                }
            }

            return false;
        }

        private static bool CheckCycleForChildren(DependencyLeaf children, DependencyLeaf parent, int maxDepth, ISet<DependencyLeaf> visitedChildren, int currentDepth = 0)
        {
            if (children == parent)
                return true;

            foreach (var child in children.Children)
            {
                if (visitedChildren.Contains(child))
                    continue;

                if (CheckCycleForChildren(child, parent, maxDepth, visitedChildren, currentDepth + 1))
                    return true;
                visitedChildren.Add(child);
            }

            return false;
        }

        public override string ToString() => $"{Item.Name} loads Before: {string.Join(", ", Children.Select(x => x.Item.Name))} and loads After: {string.Join(", ", Parents.Select(x => x.Item.Name))} (BeforeCount: {Children.Count}, AfterCount: {Parents.Count})";
    }
}