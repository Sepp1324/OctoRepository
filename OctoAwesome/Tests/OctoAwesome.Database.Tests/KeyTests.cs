using System;
using NUnit.Framework;

namespace OctoAwesome.Database.Tests
{
    [TestOf(typeof(Key<>))]
    public class KeyTests
    {
        [Test]
        public void EmptyTest()
        {
            var key = Key<DemoClass>.Empty;
            var bytes = key.GetBytes();
        }

        public class DemoClass : ITag
        {
            public int Length => 0;

            public void FromBytes(byte[] array, int startIndex)
            {
            }

            public byte[] GetBytes()
            {
                return Array.Empty<byte>();
            }
        }
    }
}