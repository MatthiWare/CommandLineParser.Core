using MatthiWare.CommandLine.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Parsing.Resolvers
{
    public class GlobalParserTests : TestBase
    {
        public GlobalParserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData(new string[] { "-int-arr", "1", "2", "3" }, true)]
        [InlineData(new string[] { "-int-arr", "1", "-int-arr", "2", "-int-arr", "3" }, true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Compiler Error")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Compiler Error")]
        public void ParseIntArray(string[] args, bool avoidCompilerError)
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(args);

            result.AssertNoErrors();

            Assert.Equal(1, result.Result.IntArray[0]);
            Assert.Equal(2, result.Result.IntArray[1]);
            Assert.Equal(3, result.Result.IntArray[2]);
        }

        [Theory]
        [InlineData(new string[] { "-str-arr", "1", "2", "3" }, true)]
        [InlineData(new string[] { "-str-arr", "1", "-str-arr", "2", "-str-arr", "3" }, true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Compiler Error")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Compiler Error")]
        public void ParseStringArray(string[] args, bool avoidCompilerError)
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(args);

            result.AssertNoErrors();

            Assert.Equal("1", result.Result.StrArray[0]);
            Assert.Equal("2", result.Result.StrArray[1]);
            Assert.Equal("3", result.Result.StrArray[2]);
        }

        [Theory]
        [InlineData(new string[] { "-int-list", "1", "2", "3" }, true)]
        [InlineData(new string[] { "-int-list", "1", "-int-list", "2", "-int-list", "3" }, true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Compiler Error")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Compiler Error")]
        public void ParseIntList(string[] args, bool avoidCompilerError)
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(args);

            result.AssertNoErrors();

            Assert.Equal(1, result.Result.IntList[0]);
            Assert.Equal(2, result.Result.IntList[1]);
            Assert.Equal(3, result.Result.IntList[2]);
        }

        private class CollectionModel
        {
            [Name("int-arr")]
            public int[] IntArray { get; set; }

            [Name("str-arr")]
            public string[] StrArray { get; set; }

            [Name("int-list")]
            public List<int> IntList { get; set; }
        }
    }
}
