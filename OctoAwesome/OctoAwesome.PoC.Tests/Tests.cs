using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.PoC.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var dependencies = new List<DependencyItem>()
            {
                new(typeof(string), "String", new(), new()),
                new(typeof(int), "Int", new() { "Bool", "ULong" }, new()),
                new(typeof(bool), "Bool", new(), new() { "Long", "UInt" }),
                new(typeof(long), "Long", new(), new() { "ULong" }),
                new(typeof(uint), "UInt", new() { "ULong" }, new()),
                new(typeof(ulong), "ULong", new() { "String" }, new())
            };

            // String -> Bool -> Long -> ULong -> Uint -> Int

            //var root = new List<DependencyItem>();
            //var unresolved = new List<DependencyItem>();

            //foreach (var dependency in dependencies)
            //{
            //    if (dependency.AfterDependencyItems.Count == 0 && dependency.BeforeDependencyItems.Count > 0)
            //    //if (dependency.AfterDependencyItems.Count == 0 && dependency.BeforeDependencyItems.Count == 0)
            //        root.Add(dependency);
            //    else
            //        unresolved.Add(dependency);
            //}

            //var leafs = new DependencyItem[dependencies.Count];

            //for (var i = 0; i < dependencies.Count; i++)
            //{
            //    var item = dependencies[i];
            //    var earliestPosition = 0;
            //    var latestPosition = 0;

            //    if (item.AfterDependencyItems.Count == 0 && item.BeforeDependencyItems.Count == 0)
            //    {
            //        for (var j = 0; j < leafs.Length; j++)
            //        {
            //            if (leafs[j] is not null) 
            //                continue;

            //            leafs[j] = item;
            //            break;
            //        }

            //        foreach (var parentDependency in item.AfterDependencyItems)
            //        {
            //            for (var j = 0; j < leafs.Length; j++)
            //            {
            //                if (leafs[j]?.Name != parentDependency) continue;

            //                if (earliestPosition < j)
            //                    earliestPosition = j;
            //            }
            //        }
            //    }
            //}

            var dic = dependencies.ToDictionary(x => x.Name, x => new RefCount(x, new(), new()));

            foreach (var item in dependencies)
            {
                if (!dic.TryGetValue(item.Name, out var refCount))
                    continue;

                refCount = new(item, new(), new());

                foreach (var after in item.AfterDependencyItems)
                {
                    if (!dic.TryGetValue(after, out var afterCount))
                        continue;

                    afterCount.Before.Add(refCount);
                }

                foreach (var before in item.BeforeDependencyItems)
                {
                    if (!dic.TryGetValue(before, out var beforeCount))
                        continue;

                    beforeCount.After.Add(refCount);
                }
            }


            Assert.Pass();
        }

        record RefCount(DependencyItem dependencyItem, List<RefCount> Before, List<RefCount> After)
        {
            public override string ToString() => $"{dependencyItem.Name}  Loads {Before.Count} Before;{string.Join(", ", Before.Select(x => x.dependencyItem.Name))}  Loads {After.Count} After:{string.Join(", ", After.Select(x => x.dependencyItem.Name))}";
        }
    }
}