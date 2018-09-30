using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;
using Xunit;

namespace MatthiWare.CommandLineParser.Tests.Parsing.Resolvers
{

    public class DoubleResolverTests
    {
        [Theory]
        [InlineData(true, "6E-14")]
        [InlineData(false, "false")]
        public void TestCanResolve(bool expected, string value)
        {
            var resolver = new DoubleResolver();
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
            var resolver = new DoubleResolver();
            var model = new ArgumentModel("key", value);

            Assert.Equal(expected, resolver.Resolve(model));
        }


    }
}
