using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Parsing.Resolvers
{
    public class DefaultResolverTests
        : BaseResolverTests
    {
        public DefaultResolverTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void ThrowsExceptionInCorrectPlaces()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultResolver<object>(null, null));
            Assert.Throws<ArgumentNullException>(() => new DefaultResolver<object>(NullLogger<CommandLineParser>.Instance, null));
        }

        [Fact]
        public void WorksCorrectlyWithNullValues()
        {
            var resolver = new DefaultResolver<object>(NullLogger<CommandLineParser>.Instance, ServiceProvider);

            Assert.False(resolver.CanResolve((ArgumentModel)null));
            Assert.False(resolver.CanResolve((string)null));
            Assert.Null(resolver.Resolve((ArgumentModel)null));
            Assert.Null(resolver.Resolve((string)null));
        }

        [Theory]
        [InlineData(true, "-m", "test")]
        [InlineData(true, "-m", "my string")]
        public void TestCanResolve(bool expected, string key, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<MyTestType>>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.CanResolve(model));
            Assert.Equal(expected, resolver.CanResolve(value));
        }

        [Theory]
        [InlineData(false, "-m", "test")]
        [InlineData(false, "-m", "my string")]
        public void TestCanResolveWithWrongCtor(bool expected, string key, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<MyTestType2>>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.CanResolve(model));
            Assert.Equal(expected, resolver.CanResolve(value));
        }

        [Theory]
        [InlineData("test", "-m", "test")]
        [InlineData("my string", "-m", "my string")]
        public void TestResolve(string expected, string key, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<MyTestType>>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.Resolve(model).Result);
            Assert.Equal(expected, resolver.Resolve(value).Result);
        }

        public class MyTestType
        {
            public MyTestType(string ctor)
            {
                Result = ctor;
            }

            public string Result { get; }
        }

        public class MyTestType2
        {
            public MyTestType2(int someInt)
            {
            }
        }
    }
}
