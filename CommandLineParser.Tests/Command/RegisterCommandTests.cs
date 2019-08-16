using MatthiWare.CommandLine.Abstractions.Command;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Command
{
    public class RegisterCommandTests
    {
        [Fact]
        public void RegisterCommandWithNoneCommandTypeThrowsException()
        {
            var parser = new CommandLineParser();

            Assert.Throws<ArgumentException>(() => parser.RegisterCommand(typeof(object)));
        }

        [Fact]
        public void RegisterCommandWithWrongParentTypeThrowsException()
        {
            var parser = new CommandLineParser();

            Assert.Throws<ArgumentException>(() => parser.RegisterCommand(typeof(MyWrongCommand)));
        }

        private class MyWrongCommand : Command<Exception, Exception>
        {

        }
    }
}
