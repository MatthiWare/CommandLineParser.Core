using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core.Attributes;
using Moq;
using Xunit;

namespace MatthiWare.CommandLineParser.Tests.Usage
{
    public class UsagePrinterTests
    {
        private class UsagePrinterGetsCalledOptions
        {
            [Name("o"), Required]
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
    }
}
