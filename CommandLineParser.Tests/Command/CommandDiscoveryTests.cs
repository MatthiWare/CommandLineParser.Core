using MatthiWare.CommandLine.Core.Command;
using Xunit;
using MatthiWare.CommandLine.Abstractions.Command;
using System.Reflection;

namespace MatthiWare.CommandLine.Tests.Command
{
    public class CommandDiscoveryTests
    {
        [Fact]
        public void DiscoverCommandFromAssemblyContainsCorrectTypes()
        {
            var cmdDiscovery = new CommandDiscoverer();

            var resultTypes = cmdDiscovery.DiscoverCommandTypes(typeof(object), new[] { Assembly.GetExecutingAssembly() });

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
            var parser = new CommandLineParser<CommandDiscoveryTests>();

            parser.DiscoverCommands(Assembly.GetExecutingAssembly());

            Assert.Equal(2, parser.Commands.Count);
        }

        public abstract class AbstractCommand : Command<CommandDiscoveryTests>
        {
        }

        public abstract class WrongGenericTypeCommand : Command<object>
        {
        }

        public class ValidCommand : Command<CommandDiscoveryTests>
        {
        }

        public class ValidCommand2 : Command<CommandDiscoveryTests, object>
        {  
        }
    }
}
