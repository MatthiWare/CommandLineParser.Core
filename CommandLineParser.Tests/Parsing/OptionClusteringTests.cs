using MatthiWare.CommandLine.Core.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Parsing
{
    public class OptionClusteringTests : TestBase
    {
        public OptionClusteringTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void ClusterdOptionsAreParsedCorrectly()
        {
            var parser = new CommandLineParser<ClusteredOptions<bool>>(Services);

            var result = parser.Parse(new[] { "-abc" });

            result.AssertNoErrors();

            var model = result.Result;

            Assert.True(model.A);
            Assert.True(model.B);
            Assert.True(model.C);
        }

        [Fact]
        public void ClusterdOptionsAllSetTheSameValue()
        {
            var parser = new CommandLineParser<ClusteredOptions<bool>>(Services);

            var result = parser.Parse(new[] { "-abc", "false" });

            result.AssertNoErrors();

            var model = result.Result;

            Assert.False(model.A);
            Assert.False(model.B);
            Assert.False(model.C);
        }

        [Fact]
        public void ClusterdOptionsAreIgnoredWhenRepeated()
        {
            var parser = new CommandLineParser<ClusteredOptions<bool>>(Services);

            var result = parser.Parse(new[] { "-abc", "false", "-abc", "true" });

            result.AssertNoErrors();

            var model = result.Result;

            Assert.False(model.A);
            Assert.False(model.B);
            Assert.False(model.C);
        }

        [Fact]
        public void ClusterdOptionsInCommandWork()
        {
            var parser = new CommandLineParser(Services);

            parser.AddCommand<ClusteredOptions<bool>>().Name("cmd").Required().OnExecuting((o, model) =>
            {
                Assert.False(model.A);
                Assert.False(model.B);
                Assert.False(model.C);
            });

            var result = parser.Parse(new[] { "cmd", "-abc", "false" });

            result.AssertNoErrors();
        }

        [Fact]
        public void ClusterdOptionsInCommandAndReusedInParentWork()
        {
            var parser = new CommandLineParser<ClusteredOptions<bool>>(Services);

            parser.AddCommand<ClusteredOptions<bool>>().Name("cmd").Required().OnExecuting((o, model) =>
            {
                Assert.False(model.A);
                Assert.False(model.B);
                Assert.False(model.C);
            });

            var result = parser.Parse(new[] { "-abc", "cmd", "-abc", "false" });

            result.AssertNoErrors();

            Assert.True(result.Result.A);
            Assert.True(result.Result.B);
            Assert.True(result.Result.C);
        }

        [Fact]
        public void ClusterdOptionsInCommandAndReusedInParentWork_String_Version()
        {
            var parser = new CommandLineParser<ClusteredOptions<string>>(Services);

            parser.AddCommand<ClusteredOptions<string>>().Name("cmd").Required().OnExecuting((o, model) =>
            {
                Assert.Equal("false", model.A);
                Assert.Equal("false", model.B);
                Assert.Equal("false", model.C);
            });

            var result = parser.Parse(new[] { "-abc", "works", "cmd", "-abc", "false" });

            result.AssertNoErrors();

            Assert.Equal("works", result.Result.A);
            Assert.Equal("works", result.Result.B);
            Assert.Equal("works", result.Result.C);
        }

        private class ClusteredOptions<T>
        {
            [Name("a"), Required]
            public T A { get; set; }

            [Name("b"), Required]
            public T B { get; set; }

            [Name("c"), Required]
            public T C { get; set; }
        }
    }
}
