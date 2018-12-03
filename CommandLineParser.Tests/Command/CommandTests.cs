using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatthiWare.CommandLine;
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

    }
}
