using MatthiWare.CommandLine.Core.Attributes;
using Xunit;
using Xunit.Abstractions;
using System.Linq;

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
            Services.AddCommandLineParser<ClusteredOptions<bool>>();

            var parser = ResolveParser<ClusteredOptions<bool>>();

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
            Services.AddCommandLineParser<ClusteredOptions<bool>>();

            var parser = ResolveParser<ClusteredOptions<bool>>();

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
            Services.AddCommandLineParser<ClusteredOptions<bool>>();

            var parser = ResolveParser<ClusteredOptions<bool>>();

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
            Services.AddCommandLineParser();

            var parser = ResolveParser();

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
            Services.AddCommandLineParser<ClusteredOptions<bool>>();

            var parser = ResolveParser<ClusteredOptions<bool>>();

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
            Services.AddCommandLineParser<ClusteredOptions<string>>();

            var parser = ResolveParser<ClusteredOptions<string>>();

            parser.AddCommand<ClusteredOptions<string>>().Name("cmd").Required().OnExecuting((o, model) =>
            {
                Assert.Equal("false", model.A);
                Assert.Equal("false", model.B);
                Assert.Equal("false", model.C);
            });

            var result = parser.Parse(new[] { "-abc", "works", "cmd", "-abc", "false" });

            result.AssertNoErrors();

            var cmdResult = result.CommandResults.First(cmd => cmd.Command.Name == "cmd");

            Assert.True(cmdResult.Found);
            Assert.True(cmdResult.Executed);

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
