using System.Threading;
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

            parser.Configure(opt => opt.Option1)
                .Name("-o")
                .Default("Default message")
                .Required();

            var parsed = parser.Parse(new string[] { "app.exe", "-o", "test" });

            Assert.NotNull(parsed);

            Assert.False(parsed.HasErrors);

            Assert.Equal("test", parsed.Result.Option1);
        }

        [Theory]
        [InlineData(new[] { "app.exe", "-1", "message1", "-2", "-3" }, "message1", "message2", "message3")]
        [InlineData(new[] { "app.exe", "-1", "-2", "message2", "-3" }, "message1", "message2", "message3")]
        [InlineData(new[] { "app.exe", "-1", "-2", "-3" }, "message1", "message2", "message3")]
        public void ParseWithDefaults(string[] args, string result1, string result2, string result3)
        {
            var parser = new CommandLineParser<OptionsWithThreeParams>();

            parser.Configure(opt => opt.Option1)
                .Name("-1")
                .Default(result1)
                .Required();

            parser.Configure(opt => opt.Option2)
                .Name("-2")
                .Default(result2)
                .Required();

            parser.Configure(opt => opt.Option3)
                .Name("-3")
                .Default(result3)
                .Required();

            var parsed = parser.Parse(args);

            Assert.NotNull(parsed);

            Assert.False(parsed.HasErrors);

            Assert.Equal(result1, parsed.Result.Option1);
            Assert.Equal(result2, parsed.Result.Option2);
            Assert.Equal(result3, parsed.Result.Option3);
        }

        [Fact]
        public void ParseWithCommandTests()
        {
            var wait = new ManualResetEvent(false);

            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Option1)
                .Name("-o")
                .Default("Default message")
                .Required();

            var addCmd = parser.AddCommand<AddOption>()
                .Name("-A", "--Add")
                .OnExecuting(x =>
                {
                    Assert.Equal("my message", x.Message);
                    wait.Set();
                });


            addCmd.Configure(opt => opt.Message)
                .Name("-m", "--message")
                .Required();

            var parsed = parser.Parse(new string[] { "app.exe", "-o", "test", "--Add", "-m", "my message" });

            Assert.False(parsed.HasErrors);

            Assert.NotNull(parsed);

            Assert.Equal("test", parsed.Result.Option1);

            parsed.ExecuteCommands();

            Assert.True(wait.WaitOne(2000));
        }

        [Theory]
        [InlineData(new[] { "app.exe", "--Add", "-m", "message2", "-m", "message1" }, "message1", "message2")]
        [InlineData(new[] { "app.exe", "-m", "message1", "--Add", "-m", "message2" }, "message1", "message2")]
        public void ParseCommandTests(string[] args, string result1, string result2)
        {
            var parser = new CommandLineParser<AddOption>();

            parser.AddCommand<AddOption>()
                .Name("-a", "--add")
                .Required()
                .OnExecuting(r => Assert.Equal(result2, r.Message))
                .Configure(c => c.Message)
                    .Name("-m", "--message")
                    .Required();

            parser.Configure(opt => opt.Message)
                .Name("-m", "--message")
                .Required();

            var result = parser.Parse(args);

            Assert.False(result.HasErrors);

            result.ExecuteCommands();

            Assert.Equal(result1, result.Result.Message);
        }

        [Fact]
        public void ConfigureTests()
        {
            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Option1)
                .Name("-o", "--opt")
                .Default("Default message")
                .Required();

            parser.Configure(opt => opt.Option2)
                .Name("-x", "--xsomething")
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

        private class OptionsWithThreeParams
        {
            public string Option1 { get; set; }
            public string Option2 { get; set; }
            public string Option3 { get; set; }
        }
    }
}
