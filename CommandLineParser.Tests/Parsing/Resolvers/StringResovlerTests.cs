using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;
using Xunit;

namespace MatthiWare.CommandLineParser.Tests.Parsing.Resolvers
{
    public class StringResovlerTests
    {

        [Theory]
        [InlineData(true, "-m", "test")]
        public void TestCanResolve(bool expected, string key, string value)
        {
            var resolver = new StringResolver();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.CanResolve(model));
        }

        [Theory]
        [InlineData("test", "-m", "test")]
        public void TestResolve(string expected, string key, string value)
        {
            var resolver = new StringResolver();
            var model = new ArgumentModel(key, value);

            Assert.Equal(expected, resolver.Resolve(model));
        }

    }
}
