using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Usage
{
    public class HelpDisplayCommandTests : TestBase
    {
        public HelpDisplayCommandTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
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
        public void TestHelpDisplayFiresCorrectly(string[] args, bool fires)
        {
            bool calledFlag = false;

            var usagePrinterMock = new Mock<IUsagePrinter>();

            usagePrinterMock.Setup(mock => mock.PrintUsage()).Callback(() => calledFlag = true);
            usagePrinterMock.Setup(mock => mock.PrintCommandUsage(It.IsAny<ICommandLineCommand>())).Callback(() => calledFlag = true);
            usagePrinterMock.Setup(mock => mock.PrintOptionUsage(It.IsAny<ICommandLineOption>())).Callback(() => calledFlag = true);

            Services.AddSingleton(usagePrinterMock.Object);

            var parser = new CommandLineParser<Options>(Services);

            var cmd = parser.AddCommand<CommandOptions>()
                .Name("db")
                .Description("Database commands");

            parser.Parse(args);

            Assert.Equal(fires, calledFlag);
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
            usagePrinterMock.Setup(mock => mock.PrintCommandUsage(It.IsAny<ICommandLineCommand>())).Callback(() => calledFlag = true);
            usagePrinterMock.Setup(mock => mock.PrintOptionUsage(It.IsAny<ICommandLineOption>())).Callback(() => calledFlag = true);

            Services.AddSingleton(usagePrinterMock.Object);

            var parser = new CommandLineParser<Options>(Services);

            var cmd = parser.AddCommand<CommandOptions>()
                .Name("db")
                .Description("Database commands");

            await parser.ParseAsync(args);

            Assert.Equal(fires, calledFlag);
        }

        [Fact]
        public void DisabledHelpOptionShouldNotPrintHelp()
        {
            var usagePrinterMock = new Mock<IUsagePrinter>();
            var options = new CommandLineParserOptions
            {
                EnableHelpOption = false
            };

            usagePrinterMock
                .Setup(p => p.PrintOptionUsage(It.IsAny<ICommandLineOption>()))
                .Verifiable();

            Services.AddSingleton(usagePrinterMock.Object);

            var parser = new CommandLineParser<Options>(options, Services);

            var result = parser.Parse(new[] { "-x", "--help" });

            result.AssertNoErrors();

            usagePrinterMock.Verify(p => p.PrintOptionUsage(It.IsAny<ICommandLineOption>()), Times.Never());
        }

        [Theory]
        [InlineData("custom", new string[] { "-x", "--custom" }, true)]
        [InlineData("custom", new string[] { "db", "--custom" }, true)]
        [InlineData("custom", new string[] { "db", "-o", "--custom" }, true)]
        [InlineData("ccc|custom", new string[] { "-x", "--custom" }, true)]
        [InlineData("ccc|custom", new string[] { "-x", "-ccc" }, true)]
        [InlineData("ccc|custom", new string[] { "db", "--custom" }, true)]
        [InlineData("ccc|custom", new string[] { "db", "-ccc" }, true)]
        [InlineData("ccc|custom", new string[] { "db", "-o", "--custom" }, true)]
        [InlineData("ccc|custom", new string[] { "db", "-o", "-ccc" }, true)]
        public async Task TestHelpOptionGetsParsedCorrectly(string helpOption, string[] args, bool fires)
        {
            bool calledFlag = false;

            var usagePrinterMock = new Mock<IUsagePrinter>();

            usagePrinterMock.Setup(mock => mock.PrintUsage()).Callback(() => calledFlag = true);
            usagePrinterMock.Setup(mock => mock.PrintCommandUsage(It.IsAny<ICommandLineCommand>())).Callback(() => calledFlag = true);
            usagePrinterMock.Setup(mock => mock.PrintOptionUsage(It.IsAny<ICommandLineOption>())).Callback(() => calledFlag = true);

            Services.AddSingleton(usagePrinterMock.Object);

            var options = new CommandLineParserOptions 
            { 
                HelpOptionName = helpOption 
            };

            var parser = new CommandLineParser<Options>(options, Services);

            var cmd = parser.AddCommand<CommandOptions>()
                .Name("db")
                .Description("Database commands");

            await parser.ParseAsync(args);

            Assert.Equal(fires, calledFlag);
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
