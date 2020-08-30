using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Parsing.Resolvers
{
    public class StringResovlerTests
        : BaseResolverTests
    {
        [Theory]
        [InlineData(true, "-m", "test")]
        [InlineData(true, "-m", "my string")]
        public void TestCanResolve(bool expected, string key, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<string>>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.CanResolve(model));
        }

        [Theory]
        [InlineData("test", "-m", "test")]
        [InlineData("my string", "-m", "my string")]
        public void TestResolve(string expected, string key, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<string>>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.Resolve(model));
        }
    }
}
