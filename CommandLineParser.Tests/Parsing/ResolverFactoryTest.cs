using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core;
using MatthiWare.CommandLine.Core.Parsing;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;
using Moq;
using System;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Parsing
{
    public class ResolverFactoryTest
    {
        public class RandomType { }

        public enum Output
        {
            Verbose,
            Info,
            Error
        }

        [Fact]
        public void ContainsWork()
        {
            var factory = new DefaultArgumentResolverFactory(new DefaultContainerResolver());

            Assert.True(factory.Contains<string>());
            Assert.True(factory.Contains<int>());
            Assert.True(factory.Contains<double>());
            Assert.True(factory.Contains<bool>());
            Assert.True(factory.Contains<Output>());

            Assert.False(factory.Contains<RandomType>());
        }

        [Fact]
        public void CreateEnumResolver()
        {
            var factory = new DefaultArgumentResolverFactory(new DefaultContainerResolver());

            var output = factory.CreateResolver<Output>();
            var output2 = factory.CreateResolver(typeof(Output));

            Assert.NotNull(output);
            Assert.NotNull(output2);

            Assert.Same(output, output2);
        }

        [Fact]
        public void RegisterAndGet()
        {
            var instance = new RandomType();

            var mockResolver = new Mock<BaseArgumentResolver<RandomType>>();
            mockResolver.Setup(_ => _.CanResolve(It.IsAny<ArgumentModel>())).Returns(true);
            mockResolver.Setup(_ => _.Resolve(It.IsAny<ArgumentModel>())).Returns(instance);

            var factory = new DefaultArgumentResolverFactory(new DefaultContainerResolver());

            factory.Register(mockResolver.Object);

            var resolver = factory.CreateResolver<RandomType>();

            var model = new ArgumentModel();

            Assert.Same(mockResolver.Object, resolver);
            Assert.True(resolver.CanResolve(model));
            Assert.Same(instance, resolver.Resolve(model));

            mockResolver.VerifyAll();
        }

        [Fact]
        public void RegisterOverrideWorks()
        {
            var mockResolver = new Mock<BaseArgumentResolver<string>>();

            var factory = new DefaultArgumentResolverFactory(new DefaultContainerResolver());

            factory.Register(typeof(string), mockResolver.Object.GetType(), true);
            factory.Register<string, StringResolver>(true);
            factory.Register<string, StringResolver>(true);
        }

        [Fact]
        public void RegisterThrowsException()
        {
            var mockResolver = new Mock<BaseArgumentResolver<string>>();

            var factory = new DefaultArgumentResolverFactory(new DefaultContainerResolver());

            Assert.Throws<ArgumentException>(() => factory.Register<string, StringResolver>());
        }

        [Fact]
        public void NonAssignableTypeThrowsException()
        {
            var factory = new DefaultArgumentResolverFactory(new DefaultContainerResolver());

            Assert.Throws<InvalidCastException>(() => factory.Register(typeof(string), typeof(Mock), true));
        }

        [Fact]
        public void RegisterObjectResolver()
        {
            var resolver = new Mock<BaseArgumentResolver<object>>();

            var obj = new object();

            resolver.Setup(_ => _.CanResolve(It.IsAny<ArgumentModel>())).Returns(true);
            resolver.Setup(_ => _.Resolve(It.IsAny<ArgumentModel>())).Returns(obj);

            var factory = new DefaultArgumentResolverFactory(new DefaultContainerResolver());
            var dummyArg = new ArgumentModel();

            factory.Register(resolver.Object);

            var createdResolver_1 = factory.CreateResolver(typeof(object));
            var createdResolver_2 = factory.CreateResolver<object>();

            Assert.NotNull(createdResolver_1);
            Assert.NotNull(createdResolver_2);

            Assert.True(createdResolver_1.CanResolve(dummyArg));
            Assert.True(createdResolver_2.CanResolve(dummyArg));

            Assert.Same(obj, createdResolver_1.Resolve(dummyArg));
            Assert.Same(obj, createdResolver_2.Resolve(dummyArg));
        }
    }
}
