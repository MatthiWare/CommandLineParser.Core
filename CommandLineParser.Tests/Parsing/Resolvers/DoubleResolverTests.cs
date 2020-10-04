using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Parsing.Resolvers
{
    public class DoubleResolverTests
        : BaseResolverTests
    {
        public DoubleResolverTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Theory]
        [InlineData(true, "6E-14")]
        [InlineData(false, "false")]
        public void TestCanResolve(bool expected, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<double>>();
            var model = new ArgumentModel("key", value);

            Assert.Equal(expected, resolver.CanResolve(model));
        }

        [Theory]
        [InlineData(5.2, "5.2")]
        [InlineData(6E-14, "6E-14")]
        [InlineData(0.84551240822557006, "0.84551240822557006")]
        [InlineData(0.84551240822557, "0.84551240822557")]
        [InlineData(4.2, "4.2000000000000002")]
        [InlineData(4.2, "4.2")]
        [InlineData(double.NaN, "NaN")]
        [InlineData(double.NegativeInfinity, "-Infinity")]
        [InlineData(double.PositiveInfinity, "Infinity")]
        public void TestResolve(double expected, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<double>>();
            var model = new ArgumentModel("key", value);

            Assert.Equal(expected, resolver.Resolve(model));
        }
    }
}
