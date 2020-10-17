using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine.Core.Command;
using MatthiWare.CommandLine.Core.Exceptions;
using MatthiWare.CommandLine.Core.Usage;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Usage
{
    public class UsagePrinterTests : TestBase
    {
        public UsagePrinterTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

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

            Services.AddSingleton(printerMock.Object);

            var parser = new CommandLineParser<Options_Issue60>(Services);

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

            Services.AddSingleton(printerMock.Object);

            var parser = new CommandLineParser<UsagePrinterGetsCalledOptions>(Services);

            parser.Parse(args);

            printerMock.Verify(mock => mock.PrintUsage(), called ? Times.Once() : Times.Never());
        }

        [Fact]
        public void UsagePrinterPrintsOptionCorrectly()
        {
            var printerMock = new Mock<IUsagePrinter>();

            Services.AddSingleton(printerMock.Object);

            var parser = new CommandLineParser<UsagePrinterGetsCalledOptions>(Services);

            parser.Parse(new[] { "-o", "--help" });

            printerMock.Verify(mock => mock.PrintOptionUsage(It.IsAny<ICommandLineOption>()), Times.Once());
        }

        [Fact]
        public void UsagePrinterPrintsCommandCorrectly()
        {
            var printerMock = new Mock<IUsagePrinter>();

            Services.AddSingleton(printerMock.Object);

            var parser = new CommandLineParser<UsagePrinterGetsCalledOptions>(Services);

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

            Services.AddSingleton(builderMock.Object);

            var parser = new CommandLineParser<UsagePrinterGetsCalledOptions>(parserOptions, Services);

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

        [Fact]
        public void TestSuggestion()
        {
            // SETUP
            string result = string.Empty;
            var expected = "'tst' is not recognized as a valid command or option.\r\n\r\nDid you mean: \r\n\tTest\r\n";

            var consoleMock = new Mock<IConsole>();
            consoleMock.Setup(_ => _.WriteLine(It.IsAny<string>())).Callback((string s) => result = s).Verifiable();

            Services.AddSingleton(consoleMock.Object);
            Services.AddSingleton(Logger);

            var parser = new CommandLineParser<OptionModel>(Services);

            var cmdConfig = parser.AddCommand<OptionModel>();
            cmdConfig.Name("ZZZZZZZZZZZZZZ").Configure(o => o.Option).Name("tst");

            parser.AddCommand().Name("Test");
            parser.Configure(o => o.Option).Name("Test1");

            var model = new UnusedArgumentModel("tst", parser);
            var printer = parser.Services.GetRequiredService<IUsagePrinter>();

            // ACT
            printer.PrintSuggestion(model);

            // ASSERT
            consoleMock.VerifyAll();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestSuggestionWithParsing()
        {
            // SETUP
            string result = string.Empty;
            var expected = "'tst' is not recognized as a valid command or option.\r\n\r\nDid you mean: \r\n\tTest\r\n";

            var consoleMock = new Mock<IConsole>();
            consoleMock.Setup(_ => _.WriteLine(It.IsAny<string>())).Callback((string s) => result += s).Verifiable();

            Services.AddSingleton(consoleMock.Object);
            Services.AddSingleton(Logger);

            var parser = new CommandLineParser<OptionModel>(Services);

            var cmdConfig = parser.AddCommand<OptionModel>();
            cmdConfig.Name("ZZZZZZZZZZZZZZ").Configure(o => o.Option).Name("tst");

            parser.AddCommand().Name("Test");
            parser.Configure(o => o.Option).Name("Test1");

            // ACT
            parser.Parse(new[] { "tst" }).AssertNoErrors();

            // ASSERT
            consoleMock.VerifyAll();
            Assert.Contains(expected, result);
        }

        private Times ToTimes(bool input)
            => input ? Times.Once() : Times.Never();

        private class OptionModel
        {
            public string Option { get; set; }
        }
    }
}
