using System.Linq;
using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using Xunit;

namespace MatthiWare.CommandLineParser.Tests.Command
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

            Assert.NotNull(parser.Commands.First(cmd => cmd.ShortName.Equals("x")));
            Assert.NotNull(parser.Commands.First(cmd => cmd.ShortName.Equals("y")));
        }



        private class MyComand : Command<object, object>
        {
            public override void OnConfigure(ICommandConfigurationBuilder<object, object> builder)
            {
                base.OnConfigure(builder);

                builder.Name("-bla").Required();
            }

            public override void OnExecute(object options, object commandOptions)
            {
                base.OnExecute(options, commandOptions);
            }
        }

    }
}
