using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;

using Xunit;

namespace MatthiWare.CommandLineParser.Tests.Parsing.Resolvers
{
    public class IntResolverTests
    {
        [Theory]
        [InlineData(true, "-m", "5")]
        [InlineData(false, "-m", "false")]
        public void TestCanResolve(bool expected, string key, string value)
        {
            var resolver = new IntResolver();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.CanResolve(model));
        }

        [Theory]
        [InlineData(5, "-m", "5")]
        [InlineData(-5, "-m", "-5")]
        public void TestResolve(int expected, string key, string value)
        {
            var resolver = new IntResolver();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.Resolve(model));
        }
    }
}
