using MatthiWare.CommandLine.Core.Command;
using Xunit;
using MatthiWare.CommandLine.Abstractions.Command;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using MatthiWare.CommandLine.Abstractions.Usage;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using TestAssembly;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Tests.Command
{
    public class CommandDiscoveryTests : TestBase
    {
        public CommandDiscoveryTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void DiscoverCommandFromAssemblyContainsCorrectTypes()
        {
            var cmdDiscovery = new CommandDiscoverer();

            var resultTypes = cmdDiscovery.DiscoverCommandTypes(typeof(SomeBaseType), new[] { Assembly.GetExecutingAssembly() });

            var invalidAbstractCommand = typeof(AbstractCommand);
            var wrongGenericTypeCommand = typeof(WrongGenericTypeCommand);
            var validCommand = typeof(ValidCommand);
            var validCommand2 = typeof(ValidCommand2);

            Assert.DoesNotContain(invalidAbstractCommand, resultTypes);
            Assert.DoesNotContain(wrongGenericTypeCommand, resultTypes);
            Assert.Contains(validCommand, resultTypes);
            Assert.Contains(validCommand2, resultTypes);
        }

        [Fact]
        public void DiscoveredCommandsAreRegisteredCorrectly()
        {
            var parser = new CommandLineParser<SomeBaseType>(Services);

            parser.DiscoverCommands(Assembly.GetExecutingAssembly());

            Assert.Equal(3, parser.Commands.Count);
        }

        [Fact]
        public async Task CommandDiscoveryWithInjectedServices()
        {
            var envMock = new Mock<IEnvironmentVariablesService>();
            envMock
                .SetupGet(_ => _.NoColorRequested)
                .Returns(true);

            var myServiceMock = new Mock<IMyService>();
            myServiceMock
                .Setup(_ => _.Call())
                .Verifiable();

            Services.AddSingleton(envMock.Object);
            Services.AddSingleton(myServiceMock.Object);

            var parser = new CommandLineParser<MyCommandWithInjectionsOptions>(Services);

            parser.DiscoverCommands(Assembly.GetExecutingAssembly());

            var result = await parser.ParseAsync(new[] { "cmd" });

            myServiceMock.Verify(_ => _.Call(), Times.Once());
        }

        [Fact]
        public async Task NonGenericCommandCanBeDiscovered()
        {
            var argResolverMock = new Mock<IArgumentResolver<NonGenericDiscoverableCommand>>();
            argResolverMock
                .Setup(_ => _.CanResolve(It.IsAny<ArgumentModel>()))
                .Verifiable();

            Services.AddSingleton(argResolverMock.Object);

            var parser = new CommandLineParser(Services);

            parser.DiscoverCommands(typeof(NonGenericDiscoverableCommand).Assembly);

            var result = await parser.ParseAsync(new[] { "cmd" });

            Assert.True(parser.Commands.Count == 1);

            argResolverMock.Verify(_ => _.CanResolve(It.IsAny<ArgumentModel>()), Times.Once());
        }

        public abstract class AbstractCommand : Command<SomeBaseType>
        {
        }

        public class WrongGenericTypeCommand : Command<object>
        {
        }

        public class ValidCommand : Command<SomeBaseType>
        {
        }

        public class ValidCommand2 : Command<SomeBaseType, object>
        {
        }

        public class MyCommandWithInjectionsOptions 
        {
        }

        public class SomeBaseType
        {
        }

        public interface IMyService
        {
            void Call();
        }

        public class MyCommandWithInjections : Command<MyCommandWithInjectionsOptions>
        {
            private readonly IMyService service;
            private readonly IUsagePrinter usagePrinter;
            private readonly IEnvironmentVariablesService environment;

            public MyCommandWithInjections(IMyService service, IUsagePrinter usagePrinter, IEnvironmentVariablesService environment)
            {
                this.service = service ?? throw new System.ArgumentNullException(nameof(service));
                this.usagePrinter = usagePrinter ?? throw new System.ArgumentNullException(nameof(usagePrinter));
                this.environment = environment ?? throw new System.ArgumentNullException(nameof(environment));
            }

            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                builder
                    .Name("cmd")
                    .AutoExecute(true);
            }

            public override Task OnExecuteAsync(MyCommandWithInjectionsOptions options, CancellationToken cancellationToken)
            {
                service.Call();
                usagePrinter.PrintUsage();

                Assert.True(environment.NoColorRequested);

                return base.OnExecuteAsync(options, cancellationToken);
            }
        }
    }
}
