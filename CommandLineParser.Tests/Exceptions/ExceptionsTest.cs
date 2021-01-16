using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Exceptions
{
    public class ExceptionsTest : TestBase
    {
        public ExceptionsTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void SubCommandNotFoundTest()
        {
            var parser = new CommandLineParser<Options2>(Services);

            var result = parser.Parse(new string[] { "cmd" });

            Assert.True(result.HasErrors);

            Assert.IsType<CommandNotFoundException>(result.Errors.First());

            Assert.Same(parser.Commands.First(), result.Errors.Cast<CommandNotFoundException>().First().Command);
        }

        [Fact]
        public async Task SubCommandNotFoundTestAsync()
        {
            var parser = new CommandLineParser<Options2>(Services);

            var result = await parser.ParseAsync(new string[] { "cmd" });

            Assert.True(result.HasErrors);

            Assert.IsType<CommandNotFoundException>(result.Errors.First());

            Assert.Same(parser.Commands.First(), result.Errors.Cast<CommandNotFoundException>().First().Command);
        }

        [Fact]
        public void CommandNotFoundTest()
        {
            Services.AddCommandLineParser();

            var parser = ResolveParser();

            parser.AddCommand().Name("missing").Required();

            var result = parser.Parse(new string[] { });

            Assert.True(result.HasErrors);

            Assert.IsType<CommandNotFoundException>(result.Errors.First());

            Assert.Same(parser.Commands.First(), result.Errors.Cast<CommandNotFoundException>().First().Command);
        }

        [Fact]
        public async Task CommandNotFoundTestAsync()
        {
            Services.AddCommandLineParser();

            var parser = ResolveParser();

            parser.AddCommand().Name("missing").Required();

            var result = await parser.ParseAsync(new string[] { });

            Assert.True(result.HasErrors);

            Assert.IsType<CommandNotFoundException>(result.Errors.First());

            Assert.Same(parser.Commands.First(), result.Errors.Cast<CommandNotFoundException>().First().Command);
        }

        [Fact]
        public async Task OptionNotFoundTestAsync()
        {
            var parser = new CommandLineParser<Options>(Services);

            var result = await parser.ParseAsync(new string[] { });

            Assert.True(result.HasErrors);

            Assert.IsType<OptionNotFoundException>(result.Errors.First());

            Assert.Same(parser.Options.First(), result.Errors.Cast<OptionNotFoundException>().First().Option);
        }

        [Fact]
        public void CommandParseExceptionTest()
        {
            Services.AddCommandLineParser();

            var parser = ResolveParser();

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
            Services.AddCommandLineParser();

            var parser = ResolveParser();

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
        public async Task CommandParseException_Prints_Errors()
        {
            var printerMock = new Mock<IUsagePrinter>();

            Services.AddSingleton(printerMock.Object);

            var parser = new CommandLineParser<OtherOptions>(Services);

            parser.AddCommand<Options>()
                .Name("missing")
                .Required()
                .Configure(opt => opt.MissingOption)
                .Name("o")
                .Required();

            var result = await parser.ParseAsync(new string[] { "-a", "1", "-b", "2", "-a", "10" ,"20" ,"30", "missing" });

            printerMock.Verify(_ => _.PrintErrors(It.IsAny<IReadOnlyCollection<Exception>>()));
        }

        [Fact]
        public void CommandParseException_Should_Contain_Correct_Message_Single()
        {
            var cmdMock = new Mock<ICommandLineCommand>();
            cmdMock.SetupGet(_ => _.Name).Returns("test");

            var exceptionList = new List<Exception>
            {
                new Exception("msg1")
            };

            var parseException = new CommandParseException(cmdMock.Object, exceptionList.AsReadOnly());
            var msg = parseException.Message;
            var expected = @"Unable to parse command 'test' reason: msg1";

            Assert.Equal(expected, msg);
        }

        [Fact]
        public void CommandParseException_Should_Contain_Correct_Message_Multiple()
        {
            var cmdMock = new Mock<ICommandLineCommand>();
            cmdMock.SetupGet(_ => _.Name).Returns("test");

            var exceptionList = new List<Exception>
            {
                new Exception("msg1"),
                new Exception("msg2")
            };

            var parseException = new CommandParseException(cmdMock.Object, exceptionList.AsReadOnly());
            var msg = parseException.Message;
            var expected = @"Unable to parse command 'test' because 2 errors occured
 - msg1
 - msg2
";

            Assert.Equal(expected, msg);
        }

        [Fact]
        public void OptionParseExceptionTest()
        {
            var parser = new CommandLineParser<Options>(Services);

            var result = parser.Parse(new string[] { "-m", "bla" });

            Assert.True(result.HasErrors);

            Assert.IsType<OptionParseException>(result.Errors.First());

            Assert.Same(parser.Options.First(), result.Errors.Cast<OptionParseException>().First().Option);
        }

        [Fact]
        public async Task OptionParseExceptionTestAsync()
        {
            var parser = new CommandLineParser<Options>(Services);

            var result = await parser.ParseAsync(new string[] { "-m", "bla" });

            Assert.True(result.HasErrors);

            Assert.IsType<OptionParseException>(result.Errors.First());

            Assert.Same(parser.Options.First(), result.Errors.Cast<OptionParseException>().First().Option);
        }

        private class OtherOptions
        {
            [Required, Name("a")]
            public int A { get; set; }

            [Required, Name("b")]
            public int B { get; set; }
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
