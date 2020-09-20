using MatthiWare.CommandLine.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Core
{
    public class TypedInstanceCacheTests : TestBase
    {
        public TypedInstanceCacheTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddingItemsDoesNotTriggerResolve(bool doubleAdd)
        {
            var containerMock = new Mock<IServiceProvider>();

            var cache = new TypedInstanceCache<MyType>(containerMock.Object);

            var type1 = new MyType();
            var type2 = new MyType();

            cache.Add(type1);

            var result = cache.Get();

            Assert.Equal(type1, result.First());

            if (doubleAdd)
            {
                cache.Add(type2);

                result = cache.Get();

                Assert.Equal(type2, result.First());
            }

            Assert.True(result.Count == 1);

            containerMock.Verify(c => c.GetService(It.Is<Type>(t => t == typeof(MyType))), Times.Never());
        }

        [Fact]
        public void AddingItemTypeDoesTriggerResolve()
        {
            var type1 = new MyType();

            Services.AddSingleton(type1);

            var cache = new TypedInstanceCache<MyType>(Services.BuildServiceProvider());

            cache.Add(typeof(MyType));

            var result = cache.Get();

            Assert.Equal(type1, result.First());

            Assert.True(result.Count == 1);
        }

        private class MyType { }
    }
}
