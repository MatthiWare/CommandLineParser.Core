using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine.Core.Exceptions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Exceptions
{
    public class ExceptionsTest
    {
        [Fact]
        public void SubCommandNotFoundTest()
        {
            var parser = new CommandLineParser<Options2>();

            var result = parser.Parse(new string[] { "cmd" });

            Assert.True(result.HasErrors);

            Assert.IsType<CommandNotFoundException>(result.Errors.First());

            Assert.Same(parser.Commands.First(), result.Errors.Cast<CommandNotFoundException>().First().Command);
        }

        [Fact]
        public async Task SubCommandNotFoundTestAsync()
        {
            var parser = new CommandLineParser<Options2>();

            var result = await parser.ParseAsync(new string[] { "cmd" });

            Assert.True(result.HasErrors);

            Assert.IsType<CommandNotFoundException>(result.Errors.First());

            Assert.Same(parser.Commands.First(), result.Errors.Cast<CommandNotFoundException>().First().Command);
        }

        [Fact]
        public void CommandNotFoundTest()
        {
            var parser = new CommandLineParser();

            parser.AddCommand().Name("missing").Required();

            var result = parser.Parse(new string[] { });

            Assert.True(result.HasErrors);

            Assert.IsType<CommandNotFoundException>(result.Errors.First());

            Assert.Same(parser.Commands.First(), result.Errors.Cast<CommandNotFoundException>().First().Command);
        }

        [Fact]
        public async Task CommandNotFoundTestAsync()
        {
            var parser = new CommandLineParser();

            parser.AddCommand().Name("missing").Required();

            var result = await parser.ParseAsync(new string[] { });

            Assert.True(result.HasErrors);

            Assert.IsType<CommandNotFoundException>(result.Errors.First());

            Assert.Same(parser.Commands.First(), result.Errors.Cast<CommandNotFoundException>().First().Command);
        }

        [Fact]
        public async Task OptionNotFoundTestAsync()
        {
            var parser = new CommandLineParser<Options>();

            var result = await parser.ParseAsync(new string[] { });

            Assert.True(result.HasErrors);

            Assert.IsType<OptionNotFoundException>(result.Errors.First());

            Assert.Same(parser.Options.First(), result.Errors.Cast<OptionNotFoundException>().First().Option);
        }

        [Fact]
        public void CommandParseExceptionTest()
        {
            var parser = new CommandLineParser();

            parser.AddCommand<Options>()
                .Name("missing")
                .Required()
                .Configure(opt => opt.MissingOption)
                .Name("o")
                .Required();

            var result = parser.Parse(new string[] { "missing", "-o", "bla" });

            Assert.True(result.HasErrors);

            Assert.IsType<CommandParseException>(result.Errors.First());

            Assert.Same(parser.Commands.First(), result.Errors.Cast<CommandParseException>().First().Command);
        }

        [Fact]
        public async Task CommandParseExceptionTestAsync()
        {
            var parser = new CommandLineParser();

            parser.AddCommand<Options>()
                .Name("missing")
                .Required()
                .Configure(opt => opt.MissingOption)
                .Name("o")
                .Required();

            var result = await parser.ParseAsync(new string[] { "missing", "-o", "bla" });

            Assert.True(result.HasErrors);

            Assert.IsType<CommandParseException>(result.Errors.First());

            Assert.Same(parser.Commands.First(), result.Errors.Cast<CommandParseException>().First().Command);
        }

        [Fact]
        public void OptionParseExceptionTest()
        {
            var parser = new CommandLineParser<Options>();

            var result = parser.Parse(new string[] { "-m", "bla" });

            Assert.True(result.HasErrors);

            Assert.IsType<OptionParseException>(result.Errors.First());

            Assert.Same(parser.Options.First(), result.Errors.Cast<OptionParseException>().First().Option);
        }

        [Fact]
        public async Task OptionParseExceptionTestAsync()
        {
            var parser = new CommandLineParser<Options>();

            var result = await parser.ParseAsync(new string[] { "-m", "bla" });

            Assert.True(result.HasErrors);

            Assert.IsType<OptionParseException>(result.Errors.First());

            Assert.Same(parser.Options.First(), result.Errors.Cast<OptionParseException>().First().Option);
        }

        private class Options
        {
            [Required, Name("m", "missing")]
            public int MissingOption { get; set; }
        }

        private class Options2
        {
            public SubCmd SubCmd { get; set; }
        }

        private class Cmd : Command<Options2>
        {
            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                base.OnConfigure(builder);

                builder.Name("cmd").Required();
            }
        }

        private class SubCmd : Command<Options2>
        {
            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                base.OnConfigure(builder);

                builder.Name("sub").Required();
            }
        }
    }
}
