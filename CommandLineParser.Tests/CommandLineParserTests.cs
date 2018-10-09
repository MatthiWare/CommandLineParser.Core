using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MatthiWare.CommandLine;
using Xunit;
using static MatthiWare.CommandLineParser.Tests.XUnitExtensions;

namespace MatthiWare.CommandLineParser.Tests
{
    public class CommandLineParserTests
    {
        [Fact]
        public void ParseTests()
        {
            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Option1)
                .ShortName("-o")
                .Default("Default message")
                .Required();

            var parsed = parser.Parse(new string[] { "app.exe", "-o", "test" });

            Assert.NotNull(parsed);

            Assert.False(parsed.HasErrors);

            Assert.Equal("test", parsed.Result.Option1);
        }

        [Fact]
        public void ParseWithCommandTests()
        {
            var wait = new ManualResetEvent(false);

            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Option1)
                .ShortName("-o")
                .Default("Default message")
                .Required();

            bool set = false;

            var addCmd = parser.AddCommand<AddOption>()
                .ShortName("-A")
                .LongName("--Add")
                .OnExecuting(x =>
                {
                    Assert.Equal("my message", x.Message);
                    set = true;
                    wait.Set();
                });


            addCmd.Configure(opt => opt.Message)
                .LongName("--message")
                .ShortName("-m")
                .Required();

            var parsed = parser.Parse(new string[] { "app.exe", "-o", "test", "--Add", "-m", "my message" });

            Assert.False(parsed.HasErrors);

            Assert.NotNull(parsed);

            Assert.Equal("test", parsed.Result.Option1);

            Assert.True(wait.WaitOne(2000));
        }

        [Fact]
        public void ConfigureTests()
        {
            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Option1)
                .ShortName("-o")
                .LongName("--opt")
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

            Assert.Equal("-o", message.ShortName);
            Assert.Equal("--opt", message.LongName);
            Assert.True(message.HasDefault);

            Assert.Equal("-x", option.ShortName);
            Assert.Equal("--xsomething", option.LongName);
            Assert.False(option.HasDefault);
        }

        private class AddOption
        {
            public string Message { get; set; }
        }
        private class Options
        {
            public string Option1 { get; set; }
            public bool Option2 { get; set; }
        }
    }
}
