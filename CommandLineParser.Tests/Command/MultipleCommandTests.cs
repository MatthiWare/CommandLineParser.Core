using MatthiWare.CommandLine.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
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
            var services = new ServiceCollection();
            services.AddSingleton(Logger);

            var parser = new CommandLineParser(services);

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
