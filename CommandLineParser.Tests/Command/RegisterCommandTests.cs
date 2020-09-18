using MatthiWare.CommandLine.Abstractions.Command;
using System;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Command
{
    public class RegisterCommandTests : TestBase
    {
        private readonly CommandLineParser parser;

        public RegisterCommandTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            parser = new CommandLineParser(Services);
        }

        [Fact]
        public void RegisterCommandWithNoneCommandTypeThrowsException()
        {
            Assert.Throws<ArgumentException>(() => parser.RegisterCommand(typeof(object)));
        }

        [Fact]
        public void RegisterCommandWithWrongParentTypeThrowsException()
        {
            Assert.Throws<ArgumentException>(() => parser.RegisterCommand(typeof(MyWrongCommand)));
        }

        private class MyWrongCommand : Command<Exception, Exception>
        {
        }
    }
}
