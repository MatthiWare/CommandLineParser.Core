using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests
{
    public abstract class TestBase 
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ILogger<CommandLineParser> Logger { get; set; }
        public IServiceCollection Services { get; set; }

        public TestBase(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
            Logger = this.testOutputHelper.BuildLoggerFor<CommandLineParser>();

            Services = new ServiceCollection();
            Services.AddSingleton(Logger);
        }
    }
}
