using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Core.Attributes;
using Xunit;

namespace MatthiWare.CommandLineParser.Tests.Command
{
    public class MultipleCommandTests
    {
        [Theory]
        [InlineData(new string[] { "cmd1", "-x", "8" }, false)]
        [InlineData(new string[] { "cmd2", "-x", "8" }, false)]
        [InlineData(new string[] { }, false)]
        public void NonRequiredCommandShouldNotSetResultInErrorStateWhenRequiredOptionsAreMissing(string[] args, bool hasErrors)
        {
            var parser = new CommandLineParser<object>();

            parser.AddCommand<MultipleCOmmandTestsOptions>()
                .Name("cmd1")
                .Required(false)
                .Description("cmd1");

            parser.AddCommand<MultipleCOmmandTestsOptions>()
                .Name("cmd2")
                .Required(false)
                .Description("cmd2");

            var result = parser.Parse(args);

            Assert.False(result.HasErrors);
        }

        private class MultipleCOmmandTestsOptions
        {
            [Required, Name("x", "bla"), Description("some description")]
            public int Option { get; set; }
        }
    }
}
