using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;

using Xunit;

namespace MatthiWare.CommandLineParser.Tests.Parsing.Resolvers
{
    public class BoolResolverTests
    {
        [Theory]
        [InlineData("yes")]
        [InlineData("1")]
        [InlineData("true")]
        [InlineData("")]
        [InlineData(null)]
        public void TestResolveTrue(string input)
        {
            var resolver = new BoolResolver();

            var result = resolver.Resolve(new ArgumentModel { Value = input });

            Assert.True(result);
        }

        [Theory]
        [InlineData("no")]
        [InlineData("0")]
        [InlineData("false")]
        public void TestResolveFalse(string input)
        {
            var resolver = new BoolResolver();

            var result = resolver.Resolve(new ArgumentModel { Value = input });

            Assert.False(result);
        }
    }
}
