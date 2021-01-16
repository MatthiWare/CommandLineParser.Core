using MatthiWare.CommandLine.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests
{
    public abstract class TestBase
    {
        private readonly ITestOutputHelper testOutputHelper;
        private IServiceProvider serviceProvider = null;

        public ILogger<CommandLineParser> Logger { get; set; }

        public IServiceCollection Services { get; set; }

        public IServiceProvider ServiceProvider
        {
            get
            {
                if (serviceProvider is null)
                {
                    serviceProvider = Services.BuildServiceProvider();
                }

                return serviceProvider;
            }
        }

        public ICommandLineParser<TOption> ResolveParser<TOption>()
            where TOption : class, new()
        {
            return ServiceProvider.GetRequiredService<ICommandLineParser<TOption>>();
        }
        
        public ICommandLineParser ResolveParser()
        {
            return ServiceProvider.GetRequiredService<ICommandLineParser>();
        }

        public TestBase(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
            Logger = this.testOutputHelper.BuildLoggerFor<CommandLineParser>();

            Services = new ServiceCollection();
            Services.AddSingleton(Logger);
        }
    }
}
