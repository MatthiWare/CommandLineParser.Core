﻿using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Parsing.Resolvers
{
    public class IntResolverTests
        : BaseResolverTests
    {
        public IntResolverTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Theory]
        [InlineData(true, "-m", "5")]
        [InlineData(false, "-m", "false")]
        public void TestCanResolve(bool expected, string key, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<int>>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.CanResolve(model));
        }

        [Theory]
        [InlineData(5, "-m", "5")]
        [InlineData(-5, "-m", "-5")]
        public void TestResolve(int expected, string key, string value)
        {
            var resolver = ServiceProvider.GetRequiredService<IArgumentResolver<int>>();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.Resolve(model));
        }
    }
}
