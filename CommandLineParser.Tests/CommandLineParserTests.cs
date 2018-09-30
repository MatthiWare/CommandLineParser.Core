using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine;
using Xunit;

namespace MatthiWare.CommandLineParser.Tests
{
    public class CommandLineParserTests
    {
        [Fact]
        public void ParseTests()
        {

            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Message)
                .ShortName("-m")
                .LongName("--message")
                .Default("Default message")
                .Required();

            var parsed = parser.Parse(new string[] { "app.exe", "-m", "test" });

            Assert.NotNull(parsed);

            Assert.Equal("test", parsed.Result.Message);
        }

        [Fact]
        public void ConfigureTests()
        {
            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Message)
                .ShortName("-m")
                .LongName("--message")
                .Default("Default message")
                .Required();

            parser.Configure(opt => opt.Option2)
                .ShortName("-x")
                .LongName("--xsomething")
                .Required();

            Assert.Equal(2, parser.Options.Count);

            var message = parser.Options[0];
            var option = parser.Options[1];

            Assert.NotNull(message);
            Assert.NotNull(option);

            Assert.Equal("-m", message.ShortName);
            Assert.Equal("--message", message.LongName);
            Assert.True(message.HasDefault);

            Assert.Equal("-x", option.ShortName);
            Assert.Equal("--xsomething", option.LongName);
            Assert.False(option.HasDefault);
        }

        private class Options
        {
            public string Message { get; set; }
            public bool Option2 { get; set; }
        }
    }
}
