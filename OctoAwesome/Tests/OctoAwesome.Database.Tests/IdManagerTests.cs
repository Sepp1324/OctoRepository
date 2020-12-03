using NUnit.Framework;

namespace OctoAwesome.Database.Tests
{
    [TestOf(typeof(IdManager))]
    public class IdManagerTests
    {
        [TestCase(new[] { 0, 1, 5, 8, 3, 10, 15 }, new[] { 2, 4, 6, 7, 9, 11, 12, 13, 14 })]
        [TestCase(new[] { 0, 1, 5, 5, 8, 3, 10, 15 }, new[] { 2, 4, 6, 7, 9, 11, 12, 13, 14 })]
        [TestCase(new int[0], new[] { 0 })]
        [TestCase(new[] { -1, 2 }, new[] { 0, 1 })]
        [TestCase(null, new[] { 0 })]
        public void InitIdManager(int[] ids, int[] expected)
        {
            var manager = new IdManager(ids);

            foreach (var t in expected)
                Assert.AreEqual(t, manager.GetId());
        }
    }
}
