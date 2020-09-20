using MatthiWare.CommandLine.Abstractions.Command;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Command
{
    public class CommandTests : TestBase
    {
        private readonly CommandLineParser parser;

        public CommandTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            parser = new CommandLineParser(Services);
        }

        [Fact]
        public void ConfiguringCommandsIncreasesTotalCommandInParser()
        {
            parser.AddCommand<object>().Name("x");
            parser.AddCommand<object>().Name("y");

            Assert.Equal(2, parser.Commands.Count);

            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("x")));
            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("y")));
        }

        [Fact]
        public void AddOptionLessCommand()
        {
            parser.AddCommand().Name("x");
            parser.AddCommand().Name("y");

            Assert.Equal(2, parser.Commands.Count);

            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("x")));
            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("y")));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddCommandType(bool generic)
        {
            if (generic)
            {
                parser.RegisterCommand<MyComand>();
            }
            else
            {
                parser.RegisterCommand(typeof(MyComand));
            }

            Assert.Equal(1, parser.Commands.Count);

            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("bla")));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddOtherCommandType(bool generic)
        {
            if (generic)
            {
                parser.RegisterCommand<OtherCommand>();
            }
            else
            {
                parser.RegisterCommand(typeof(OtherCommand));
            }

            Assert.Equal(1, parser.Commands.Count);

            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("other")));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddCommandTypeWithGenericOption(bool generic)
        {
            if (generic)
            {
                parser.RegisterCommand<MyComand, object>();
            }
            else
            {
                parser.RegisterCommand(typeof(MyComand), typeof(object));
            }

            Assert.Equal(1, parser.Commands.Count);

            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("bla")));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddCommandTypeWithoutGenericOption(bool generic)
        {
            if (generic)
            {
                parser.RegisterCommand<NonGenericCmd>();
            }
            else
            {
                parser.RegisterCommand(typeof(NonGenericCmd));
            }

            Assert.Equal(1, parser.Commands.Count);

            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("bla")));
        }

        private class MyComand : Command<object, object>
        {
            public override void OnConfigure(ICommandConfigurationBuilder<object> builder)
            {
                builder.Name("bla");
            }

            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                builder.Name("bla");
            }

            public override void OnExecute(object options, object commandOptions)
            {
                base.OnExecute(options, commandOptions);
            }
        }

        private class OtherCommand : Command<object>
        {
            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                builder.Name("other");
            }
        }

        private class NonGenericCmd : Abstractions.Command.Command
        {
            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                builder.Name("bla");
            }

            public override void OnExecute()
            {
                base.OnExecute();
            }
        }
    }
}
