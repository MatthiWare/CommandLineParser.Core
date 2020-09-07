﻿using MatthiWare.CommandLine.Abstractions.Command;
using Microsoft.Extensions.DependencyInjection;
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
            var services = new ServiceCollection();
            services.AddSingleton(Logger);

            parser = new CommandLineParser(services);
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
