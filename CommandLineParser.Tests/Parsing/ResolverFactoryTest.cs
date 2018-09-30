using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Parsing;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;
using Moq;
using Xunit;

namespace MatthiWare.CommandLineParser.Tests.Parsing
{
    public class ResolverFactoryTest
    {

        private class RandomType { }

        [Fact]
        public void ContainsWork()
        {
            var factory = new ResolverFactory();

            Assert.True(factory.Contains<string>());
            Assert.True(factory.Contains<int>());
            Assert.True(factory.Contains<int>());

            Assert.False(factory.Contains<RandomType>());
        }

        [Fact]
        public void RegisterOverrideWorks()
        {
            var mockResolver = new Mock<ICommandLineArgumentResolver<string>>();

            var factory = new ResolverFactory();

            factory.Register(typeof(string), mockResolver.Object.GetType(), true);
            factory.Register<string, StringResolver>(true);
        }

        [Fact]
        public void RegisterThrowsException()
        {
            var mockResolver = new Mock<ICommandLineArgumentResolver<string>>();

            var factory = new ResolverFactory();

            Assert.Throws<ArgumentException>(() => factory.Register<string, StringResolver>());
        }

    }
}
