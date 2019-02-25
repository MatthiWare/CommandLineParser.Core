using System.Linq;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine.Core.Exceptions;
using Moq;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Usage
{
    public class UsagePrinterTests
    {
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

            printerMock.Verify(mock => mock.PrintUsage(It.IsAny<IArgument>()), Times.Once());
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

            printerMock.Verify(mock => mock.PrintUsage(It.IsAny<IArgument>()), Times.Once());
        }

        [Theory]
        [InlineData(new string[] { "-o", "bla", "cmd" }, true)]
        [InlineData(new string[] { "-o", "bla", "cmd", "-x", "bla" }, false)]
        [InlineData(new string[] { "cmd", "-x", "bla" }, true)]
        public void CustomInvokedPrinterWorksCorrectly(string[] args, bool argPassed)
        {
            var printerMock = new Mock<IUsagePrinter>();

            var parserOptions = new CommandLineParserOptions
            {
                AutoPrintUsageAndErrors = false
            };

            var parser = new CommandLineParser<UsagePrinterGetsCalledOptions>(parserOptions)
            {
                Printer = printerMock.Object,
            };

            parser.AddCommand<UsagePrinterCommandOptions>()
                .Name("cmd")
                .Required();

            var result = parser.Parse(args);

            // make sure it didn't print
            printerMock.Verify(mock => mock.PrintUsage(It.IsAny<ICommandLineCommand>()), Times.Never());
            printerMock.Verify(mock => mock.PrintUsage(It.IsAny<ICommandLineOption>()), Times.Never());
            printerMock.Verify(mock => mock.PrintUsage(It.IsAny<IArgument>()), Times.Never());
            printerMock.Verify(mock => mock.PrintUsage(), Times.Never());

            if (result.HelpRequested)
                parser.Printer.PrintUsage(result.HelpRequestedFor);

            if (result.HasErrors)
            {
                foreach (var err in result.Errors)
                {
                    if (!(err is BaseParserException baseParserException)) continue;

                    parser.Printer.PrintUsage(baseParserException.Argument);
                }
            }

            printerMock.Verify(mock => mock.PrintUsage(), ToTimes(result.HelpRequested));
            printerMock.Verify(mock => mock.PrintUsage(It.IsAny<IArgument>()), ToTimes(argPassed));
        }

        private Times ToTimes(bool input)
            => input ? Times.Once() : Times.Never();
    }
}
