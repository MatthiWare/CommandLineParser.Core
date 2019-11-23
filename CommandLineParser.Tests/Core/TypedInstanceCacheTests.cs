using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Core
{
    public class TypedInstanceCacheTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddingItemsDoesNotTriggerResolve(bool doubleAdd)
        {
            var containerMock = new Mock<IContainerResolver>();

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

            containerMock.Verify(c => c.Resolve(It.Is<Type>(t => t == typeof(MyType))), Times.Never());
        }

        [Fact]
        public void AddingItemTypeDoesTriggerResolve()
        {
            var containerMock = new Mock<IContainerResolver>();

            var cache = new TypedInstanceCache<MyType>(containerMock.Object);

            var type1 = new MyType();

            containerMock.Setup(c => c.Resolve(It.Is<Type>(t => t == typeof(MyType)))).Returns(type1);

            cache.Add(typeof(MyType));

            var result = cache.Get();

            Assert.Equal(type1, result.First());

            Assert.True(result.Count == 1);

            containerMock.Verify(c => c.Resolve(It.Is<Type>(t => t == typeof(MyType))), Times.Once());
        }

        private class MyType { }
    }
}
