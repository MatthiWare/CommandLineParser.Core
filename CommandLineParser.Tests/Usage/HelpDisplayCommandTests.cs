using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core.Attributes;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Usage
{
    public class HelpDisplayCommandTests
    {
        [Theory]
        [InlineData(new string[] { "db", "--help" }, true)]
        [InlineData(new string[] { "db" }, true)]
        [InlineData(new string[] { "db", "-o", "--help" }, true)]
        [InlineData(new string[] { "db", "-o", "help" }, false)]
        [InlineData(new string[] { "-x", "blabla", "--help" }, true)]
        [InlineData(new string[] { }, true)]
        [InlineData(new string[] { "-x", "--help" }, true)]
        [InlineData(new string[] { "--help" }, true)]
        public void TestHelpDisplayFiresCorrectly(string[] args, bool fires)
        {
            bool calledFlag = false;

            var usagePrinterMock = new Mock<IUsagePrinter>();

            usagePrinterMock.Setup(mock => mock.PrintUsage()).Callback(() => calledFlag = true);
            usagePrinterMock.Setup(mock => mock.PrintUsage(It.IsAny<IArgument>())).Callback(() => calledFlag = true);
            usagePrinterMock.Setup(mock => mock.PrintUsage(It.IsAny<ICommandLineCommand>())).Callback(() => calledFlag = true);
            usagePrinterMock.Setup(mock => mock.PrintUsage(It.IsAny<ICommandLineOption>())).Callback(() => calledFlag = true);

            var parser = new CommandLineParser<Options>
            {
                Printer = usagePrinterMock.Object
            };

            var cmd = parser.AddCommand<CommandOptions>()
                .Name("db")
                .Description("Database commands");

            parser.Parse(args);

            Assert.Equal<bool>(fires, calledFlag);
        }

        [Theory]
        [InlineData(new string[] { "db", "--help" }, true)]
        [InlineData(new string[] { "db" }, true)]
        [InlineData(new string[] { "db", "-o", "--help" }, true)]
        [InlineData(new string[] { "db", "-o", "help" }, false)]
        [InlineData(new string[] { "-x", "blabla", "--help" }, true)]
        [InlineData(new string[] { }, true)]
        [InlineData(new string[] { "-x", "--help" }, true)]
        [InlineData(new string[] { "--help" }, true)]
        public async Task TestHelpDisplayFiresCorrectlyAsync(string[] args, bool fires)
        {
            bool calledFlag = false;

            var usagePrinterMock = new Mock<IUsagePrinter>();

            usagePrinterMock.Setup(mock => mock.PrintUsage()).Callback(() => calledFlag = true);
            usagePrinterMock.Setup(mock => mock.PrintUsage(It.IsAny<IArgument>())).Callback(() => calledFlag = true);
            usagePrinterMock.Setup(mock => mock.PrintUsage(It.IsAny<ICommandLineCommand>())).Callback(() => calledFlag = true);
            usagePrinterMock.Setup(mock => mock.PrintUsage(It.IsAny<ICommandLineOption>())).Callback(() => calledFlag = true);

            var parser = new CommandLineParser<Options>
            {
                Printer = usagePrinterMock.Object
            };

            var cmd = parser.AddCommand<CommandOptions>()
                .Name("db")
                .Description("Database commands");

            await parser.ParseAsync(args);

            Assert.Equal<bool>(fires, calledFlag);
        }

        public class Options
        {
            [Name("x"), Description("Some description")]
            public string Option { get; set; }
        }

        public class CommandOptions
        {
            [Name("o"), Description("Some description"), Required]
            public string Option { get; set; }
        }
    }
}
