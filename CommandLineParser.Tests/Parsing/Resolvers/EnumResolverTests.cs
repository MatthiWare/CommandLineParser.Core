using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;

using Xunit;

namespace MatthiWare.CommandLineParser.Tests.Parsing.Resolvers
{
    public class EnumResolverTests
    {
        [Theory]
        [InlineData(true, "-m", "Error")]
        [InlineData(false, "-m", "xd")]
        [InlineData(false, "-m", "")]
        public void TestCanResolve(bool expected, string key, string value)
        {
            var resolver = new EnumResolver<TestEnum>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.CanResolve(model));
        }

        [Theory]
        [InlineData(TestEnum.Error, "-m", "Error")]
        [InlineData(TestEnum.Verbose, "-m", "Verbose")]
        public void TestResolve(TestEnum expected, string key, string value)
        {
            var resolver = new EnumResolver<TestEnum>();
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
