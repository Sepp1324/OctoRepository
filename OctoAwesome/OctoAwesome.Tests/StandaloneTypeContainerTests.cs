using System;
using Xunit;

namespace OctoAwesome.Tests
{
    public class StandaloneTypeContainerTests
    {
        [Fact]
        public void InitializationTest()
        {
            new StandaloneTypeContainer();
        }

        [Fact]
        public void InstanceTest()
        {
            var typeContainer = new StandaloneTypeContainer();
            typeContainer.Register(typeof(StandaloneTypeContainer));
            typeContainer.Register(typeof(TestClass));
            typeContainer.Register(typeof(ITestInterface), typeof(TestClass));

            var result = typeContainer.TryResolve(typeof(TestClass), out var instanceA);
            Assert.True(result);

            result = typeContainer.TryResolve(typeof(TestClass), out var instanceB);
            Assert.True(result);

            result = typeContainer.TryResolve(typeof(ITestInterface), out var instanceC);
            Assert.True(result);

            result = typeContainer.TryResolve(typeof(ITestInterface), out var instanceD);
            Assert.True(result);

            Assert.True(instanceA is TestClass);
            Assert.True(instanceB is TestClass);
            Assert.True(instanceC is TestClass);
            Assert.True(instanceC is ITestInterface);
            Assert.True(instanceD is TestClass);
            Assert.True(instanceD is ITestInterface);
            Assert.NotSame(instanceD, instanceC);
            Assert.NotSame(instanceA, instanceB);
            Assert.NotSame(instanceA, instanceD);

            Assert.False(typeContainer.TryResolve(typeof(SecondTestClass), out instanceA));
            Assert.Null(instanceA);
        }

        private class TestClass : ITestInterface
        {

        }

        private class SecondTestClass
        {

        }

        private interface ITestInterface
        {

        }
    }
}
