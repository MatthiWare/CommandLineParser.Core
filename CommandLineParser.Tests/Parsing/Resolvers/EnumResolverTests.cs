using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Parsing.Resolvers
{
    public class EnumResolverTests
        : BaseResolverTests
    {
        [Theory]
        [InlineData(true, "-m", "Error")]
        [InlineData(false, "-m", "xd")]
        [InlineData(false, "-m", "")]
        public void TestCanResolve(bool expected, string key, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<TestEnum>>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.CanResolve(model));
        }

        [Theory]
        [InlineData(TestEnum.Error, "-m", "Error")]
        [InlineData(TestEnum.Error, "-m", "error")]
        [InlineData(TestEnum.Verbose, "-m", "Verbose")]
        [InlineData(TestEnum.Verbose, "-m", "verbose")]
        public void TestResolve(TestEnum expected, string key, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<TestEnum>>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.Resolve(model));
        }

        public enum TestEnum
        {
            Info,
            Error,
            Verbose
        }
    }
}
