using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine.Core.Exceptions;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Exceptions
{
    public class ExceptionsTest
    {
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
        public void OptionNotFoundTest()
        {
            var parser = new CommandLineParser<Options>();

            var result = parser.Parse(new string[] { });

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
        public void OptionParseExceptionTest()
        {
            var parser = new CommandLineParser<Options>();

            var result = parser.Parse(new string[] { "-m", "bla" });

            Assert.True(result.HasErrors);

            Assert.IsType<OptionParseException>(result.Errors.First());

            Assert.Same(parser.Options.First(), result.Errors.Cast<OptionParseException>().First().Option);
        }

        private class Options
        {
            [Required, Name("m", "missing")]
            public int MissingOption { get; set; }
        }
    }
}
