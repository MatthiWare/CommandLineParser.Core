using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core.Attributes;
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

            printerMock.Verify(mock => mock.PrintUsage(It.IsAny<ICommandLineOption>()), Times.Once());
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

            printerMock.Verify(mock => mock.PrintUsage(It.IsAny<ICommandLineCommand>()), Times.Once());
        }
    }
}
