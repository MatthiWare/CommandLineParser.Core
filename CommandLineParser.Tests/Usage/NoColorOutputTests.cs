using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine.Core.Usage;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Usage
{
    [Collection("Non-Parallel Collection")]
    public class NoColorOutputTests
    {
        private readonly CommandLineParser<Options> parser;
        private readonly IEnvironmentVariablesService variablesService;
        private Action<ConsoleColor> consoleColorGetter;
        private bool variableServiceResult;

        public NoColorOutputTests()
        {
            var envMock = new Mock<IEnvironmentVariablesService>();
            envMock.SetupGet(env => env.NoColorRequested).Returns(() => variableServiceResult);

            variablesService = envMock.Object;

            var usageBuilderMock = new Mock<IUsageBuilder>();
            usageBuilderMock.Setup(m => m.AddErrors(It.IsAny<IReadOnlyCollection<Exception>>())).Callback(() =>
            {
                consoleColorGetter(((UsagePrinter)parser.Printer).m_currentConsoleColor);
            });

            var services = new ServiceCollection();

            services.AddSingleton(envMock.Object);
            services.AddSingleton(usageBuilderMock.Object);

            parser = new CommandLineParser<Options>(services);
        }

        [Fact]
        public void CheckUsageOutputRespectsNoColor()
        {
            ParseAndCheckNoColor(false);
            ParseAndCheckNoColor(true);
        }

        private void ParseAndCheckNoColor(bool noColorOuput)
        {
            consoleColorGetter = noColorOuput ? (Action<ConsoleColor>)AssertNoColor : AssertColor;

            variableServiceResult = noColorOuput;

            parser.Parse(new string[] { "alpha" });
        }

        private void AssertNoColor(ConsoleColor color)
        {
            Assert.True(variablesService.NoColorRequested);
            Assert.NotEqual(ConsoleColor.Red, color);
        }

        private void AssertColor(ConsoleColor color)
        {
            Assert.False(variablesService.NoColorRequested);
            Assert.Equal(ConsoleColor.Red, color);
        }

        private class Options
        {
            [Required, Name("b")]
            public bool MyBool { get; set; }
        }
    }
}
