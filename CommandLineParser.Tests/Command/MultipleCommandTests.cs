using MatthiWare.CommandLine.Core.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Command
{
    public class MultipleCommandTests : TestBase
    {
        public MultipleCommandTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData(new string[] { "cmd1", "-x", "8" }, false)]
        [InlineData(new string[] { "cmd2", "-x", "8" }, false)]
        [InlineData(new string[] { }, false)]
        public void NonRequiredCommandShouldNotSetResultInErrorStateWhenRequiredOptionsAreMissing(string[] args, bool _)
        {
            Services.AddCommandLineParser();

            var parser = ResolveParser();

            parser.AddCommand<MultipleCommandTestsOptions>()
                .Name("cmd1")
                .Required(false)
                .Description("cmd1");

            parser.AddCommand<MultipleCommandTestsOptions>()
                .Name("cmd2")
                .Required(false)
                .Description("cmd2");

            var result = parser.Parse(args);

            result.AssertNoErrors();
        }

        private class MultipleCommandTestsOptions
        {
            [Required, Name("x", "bla"), Description("some description")]
            public int Option { get; set; }
        }
    }
}
