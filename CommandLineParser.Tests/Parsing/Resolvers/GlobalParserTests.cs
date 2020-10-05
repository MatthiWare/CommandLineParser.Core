using MatthiWare.CommandLine.Core.Attributes;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Parsing.Resolvers
{
    public class GlobalParserTests : TestBase
    {
        public GlobalParserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void ParseByte()
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(new[] { "-byte", "1" });

            result.AssertNoErrors();

            Assert.Equal(1, result.Result.Byte);
        }

        [Fact]
        public void ParseString()
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(new[] { "-str", "text" });

            result.AssertNoErrors();

            Assert.Equal("text", result.Result.String);
        }

        [Fact]
        public void ParseSByte()
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(new[] { "-sbyte", "1" });

            result.AssertNoErrors();

            Assert.Equal(1, result.Result.Sbyte);
        }

        [Fact]
        public void ParseInt()
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(new[] { "-int", "1" });

            result.AssertNoErrors();

            Assert.Equal(1, result.Result.Int);
        }

        [Fact]
        public void ParseLong()
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(new[] { "-long", "1" });

            result.AssertNoErrors();

            Assert.Equal(1, result.Result.Long);
        }

        [Fact]
        public void ParseULong()
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(new[] { "-ulong", "1" });

            result.AssertNoErrors();

            Assert.Equal<ulong>(1, result.Result.Ulong);
        }

        [Fact]
        public void ParseUShort()
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(new[] { "-ushort", "1" });

            result.AssertNoErrors();

            Assert.Equal<ushort>(1, result.Result.Ushort);
        }

        [Fact]
        public void ParseShort()
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(new[] { "-short", "1" });

            result.AssertNoErrors();

            Assert.Equal(1, result.Result.Short);
        }

        [Fact]
        public void ParseUint()
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(new[] { "-uint", "1" });

            result.AssertNoErrors();

            Assert.Equal<uint>(1, result.Result.Uint);
        }

        [Fact]
        public void ParseDecimal()
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(new[] { "-decimal", "1.0" });

            result.AssertNoErrors();

            Assert.Equal(1.0m, result.Result.Decimal);
        }

        [Fact]
        public void ParseFloat()
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(new[] { "-float", "1.0" });

            result.AssertNoErrors();

            Assert.Equal(1.0f, result.Result.Float);
        }

        [Fact]
        public void ParseBool()
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(new[] { "-bool", "true" });

            result.AssertNoErrors();

            Assert.True(result.Result.Bool);
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

        [Theory]
        [InlineData(new string[] { "-int-set", "1", "2", "3" }, true)]
        [InlineData(new string[] { "-int-set", "1", "-int-set", "2", "-int-set", "3" }, true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Compiler Error")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Compiler Error")]
        public void ParseIntSet(string[] args, bool avoidCompilerError)
        {
            var parser = new CommandLineParser<CollectionModel>(Services);

            var result = parser.Parse(args);

            result.AssertNoErrors();

            Assert.Contains(1, result.Result.IntSet);
            Assert.Contains(2, result.Result.IntSet);
            Assert.Contains(3, result.Result.IntSet);
        }

        private class CollectionModel
        {
            [Name("bool")]
            public bool Bool { get; set; }
            [Name("int")]
            public int Int { get; set; }
            [Name("str")]
            public string String { get; set; }
            [Name("short")]
            public short Short { get; set; }
            [Name("long")]
            public long Long { get; set; }
            [Name("double")]
            public double Double { get; set; }
            [Name("ushort")]
            public ushort Ushort { get; set; }
            [Name("ulong")]
            public ulong Ulong { get; set; }
            [Name("byte")]
            public byte Byte { get; set; }
            [Name("sbyte")]
            public sbyte Sbyte { get; set; }
            [Name("uint")]
            public uint Uint { get; set; }
            [Name("decimal")]
            public decimal Decimal { get; set; }
            [Name("float")]
            public float Float { get; set; }
            [Name("int-arr")]
            public int[] IntArray { get; set; }
            [Name("str-arr")]
            public string[] StrArray { get; set; }
            [Name("int-list")]
            public List<int> IntList { get; set; }
            [Name("int-iset")]
            public ISet<int> IntISet { get; set; }
            [Name("int-set")]
            public HashSet<int> IntSet { get; set; }
        }
    }
}
