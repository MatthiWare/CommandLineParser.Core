using System.Linq;

using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Abstractions.Command;

using Xunit;

namespace MatthiWare.CommandLine.Tests.Command
{
    public class CommandTests
    {
        [Fact]
        public void ConfiguringCommandsIncreasesTotalCommandInParser()
        {
            var parser = new CommandLineParser<object>();

            parser.AddCommand<object>().Name("x");
            parser.AddCommand<object>().Name("y");

            Assert.Equal(2, parser.Commands.Count);

            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("x")));
            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("y")));
        }

        [Fact]
        public void AddOptionLessCommand()
        {
            var parser = new CommandLineParser<object>();

            parser.AddCommand().Name("x");
            parser.AddCommand().Name("y");

            Assert.Equal(2, parser.Commands.Count);

            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("x")));
            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("y")));
        }

        [Fact]
        public void AddCommandType()
        {
            var parser = new CommandLineParser<object>();

            parser.RegisterCommand<MyComand>();

            Assert.Equal(1, parser.Commands.Count);

            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("bla")));
        }

        [Fact]
        public void AddOtherCommandType()
        {
            var parser = new CommandLineParser<object>();

            parser.RegisterCommand<OtherCommand>();

            Assert.Equal(1, parser.Commands.Count);

            Assert.NotNull(parser.Commands.First(cmd => cmd.Name.Equals("other")));
        }

        [Fact]
        public void AddCommandTypeWithGenericOption()
        {
            var parser = new CommandLineParser<object>();

            parser.RegisterCommand<MyComand, object>();

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
    }
}
