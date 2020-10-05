using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Parsing.Resolvers
{
    public class BoolResolverTests
        : BaseResolverTests
    {
        public BoolResolverTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Theory]
        [InlineData("yes")]
        [InlineData("1")]
        [InlineData("true")]
        [InlineData("")]
        [InlineData(null)]
        public void TestResolveTrue(string input)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<bool>>();

            var result = resolver.Resolve(new ArgumentModel(string.Empty, input));

            Assert.True(result);
        }

        [Theory]
        [InlineData("no")]
        [InlineData("0")]
        [InlineData("false")]
        public void TestResolveFalse(string input)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<bool>>();

            var result = resolver.Resolve(new ArgumentModel(string.Empty, input));

            Assert.False(result);
        }
    }
}
