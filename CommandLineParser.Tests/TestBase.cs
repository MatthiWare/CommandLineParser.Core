using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests
{
    public abstract class TestBase 
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ILogger Logger { get; set; }

        public TestBase(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
            Logger = this.testOutputHelper.BuildLoggerFor<CommandLineParser>();
        }
    }
}
