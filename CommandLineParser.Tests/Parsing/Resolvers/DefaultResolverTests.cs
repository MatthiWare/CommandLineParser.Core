using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Parsing.Resolvers
{
    public class DefaultResolverTests
        : BaseResolverTests
    {
        [Theory]
        [InlineData(true, "-m", "test")]
        [InlineData(true, "-m", "my string")]
        public void TestCanResolve(bool expected, string key, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<MyTestType>>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.CanResolve(model));
        }

        [Theory]
        [InlineData(false, "-m", "test")]
        [InlineData(false, "-m", "my string")]
        public void TestCanResolveWithWrongCtor(bool expected, string key, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<MyTestType2>>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.CanResolve(model));
        }

        [Theory]
        [InlineData("test", "-m", "test")]
        [InlineData("my string", "-m", "my string")]
        public void TestResolve(string expected, string key, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<MyTestType>>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.Resolve(model).Result);
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
