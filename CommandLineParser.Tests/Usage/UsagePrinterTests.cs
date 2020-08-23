using System.Linq;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine.Core.Exceptions;
using MatthiWare.CommandLine.Core.Usage;
using Moq;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Usage
{
    public class UsagePrinterTests
    {
        #region Issue_60

        private class Options_Issue60
        {
            [Name("c", "check")]
            [Description("Reports the amount of duplicates without changing anything")]
            [DefaultValue(true)] // Note defaults to true and not required
            public bool OnlyCheck { get; set; }
        }

        [Theory]
        // https://github.com/MatthiWare/CommandLineParser.Core/issues/60
        [InlineData(new string[] { }, false)]
        [InlineData(new string[] { "-c" }, false)]
        [InlineData(new string[] { "-c", "true" }, false)]
        [InlineData(new string[] { "-c", "false" }, false)]
        public void AllOptionsHaveDefaultValueShouldNotPrintUsages(string[] args, bool called)
        {
            var printerMock = new Mock<IUsagePrinter>();

            var parser = new CommandLineParser<Options_Issue60>
            {
                Printer = printerMock.Object
            };

            parser.Parse(args);

            printerMock.Verify(mock => mock.PrintUsage(), called ? Times.Once() : Times.Never());
        }

        #endregion

        private class UsagePrinterGetsCalledOptions
        {
            [Name("o"), Required]
            public string Option { get; set; }
        }

        private class UsagePrinterCommandOptions
        {
            [Name("x"), Required]
            public string Option { get; set; }
        }

        [Theory]
        [InlineData(new string[] { }, true)]
        [InlineData(new string[] { "-o", "bla" }, false)]
        [InlineData(new string[] { "-xd", "bla" }, true)]
        public void UsagePrintGetsCalledInCorrectCases(string[] args, bool called)
        {
            var printerMock = new Mock<IUsagePrinter>();

            var parser = new CommandLineParser<UsagePrinterGetsCalledOptions>
            {
                Printer = printerMock.Object
            };

            parser.Parse(args);

            printerMock.Verify(mock => mock.PrintUsage(), called ? Times.Once() : Times.Never());
        }

        [Fact]
        public void UsagePrinterPrintsOptionCorrectly()
        {
            var printerMock = new Mock<IUsagePrinter>();

            var parser = new CommandLineParser<UsagePrinterGetsCalledOptions>
            {
                Printer = printerMock.Object
            };

            parser.Parse(new[] { "-o", "--help" });

            printerMock.Verify(mock => mock.PrintOptionUsage(It.IsAny<ICommandLineOption>()), Times.Once());
        }

        [Fact]
        public void UsagePrinterPrintsCommandCorrectly()
        {
            var printerMock = new Mock<IUsagePrinter>();

            var parser = new CommandLineParser<UsagePrinterGetsCalledOptions>
            {
                Printer = printerMock.Object
            };

            parser.AddCommand<UsagePrinterCommandOptions>()
                .Name("cmd")
                .Required();

            parser.Parse(new[] { "-o", "bla", "cmd", "--help" });

            printerMock.Verify(mock => mock.PrintCommandUsage(It.IsAny<ICommandLineCommand>()), Times.Once());
        }

        [Theory]
        [InlineData(new string[] { "-o", "bla", "cmd" }, true, false)]
        [InlineData(new string[] { "-o", "bla", "cmd", "-x", "bla" }, false, false)]
        [InlineData(new string[] { "cmd", "-x", "bla" }, false, true)]
        public void CustomInvokedPrinterWorksCorrectly(string[] args, bool cmdPassed, bool optPassed)
        {
            var builderMock = new Mock<IUsageBuilder>();

            var parserOptions = new CommandLineParserOptions
            {
                AutoPrintUsageAndErrors = false
            };

            var parser = new CommandLineParser<UsagePrinterGetsCalledOptions>(parserOptions);

            parser.Printer = new UsagePrinter(parser, builderMock.Object);

            parser.AddCommand<UsagePrinterCommandOptions>()
                .Name("cmd")
                .Required();

            var result = parser.Parse(args);

            builderMock.Verify(mock => mock.Build(), Times.Never());
            builderMock.Verify(mock => mock.AddCommand(It.IsAny<string>(), It.IsAny<ICommandLineCommandContainer>()), Times.Never());
            builderMock.Verify(mock => mock.AddOption(It.IsAny<ICommandLineOption>()), Times.Never());

            if (result.HelpRequested)
            {
                parser.Printer.PrintUsage(result.HelpRequestedFor);
            }

            if (result.HasErrors)
            {
                foreach (var err in result.Errors)
                {
                    if (!(err is BaseParserException baseParserException))
                    {
                        continue;
                    }

                    parser.Printer.PrintUsage(baseParserException.Argument);
                }
            }

            builderMock.Verify(
                mock => mock.Build(),
                ToTimes(result.HelpRequested || result.HasErrors));

            builderMock.Verify(
                mock => mock.AddCommand(It.IsAny<string>(), It.IsAny<ICommandLineCommand>()),
                ToTimes(cmdPassed));

            builderMock.Verify(
                mock => mock.AddOption(It.IsAny<ICommandLineOption>()),
                ToTimes(optPassed));
        }

        private Times ToTimes(bool input)
            => input ? Times.Once() : Times.Never();
    }
}
