using System;
using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Core;
using Xunit;

namespace CommandLineParser.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Message)
                .ShortName("-m")
                .LongName("--message")
                .Default("Default message")
                .Required();

            var parsed = parser.Parse(new string[] { "app.exe", "-m", "test" });

            Assert.NotNull(parsed);

            Assert.Equal("test", parsed.Message);
        }

        [Fact]
        public void Test2()
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

        [Fact]
        public void TestV1()
        {
            var testString = "this is my test string \"with some quotes\" the end. '\"Here is some literal\"' ";

            var resultArr = new string[] { "this", "is", "my", "test", "string", "with some quotes", "the", "end.", "\"Here is some literal\"" };

            int i = 0;
            foreach (var token in Extensions.SplitOnWhitespace(testString))
            {
                Assert.Equal(resultArr[i++], token);
            }

            Assert.Equal(resultArr.Length, i);
        }

        [Theory]
        [InlineData("\"with some quotes\"", "with some quotes")]
        [InlineData("'\"with some quotes\"'", "\"with some quotes\"")]
        public void TestRemoveLiteralAndDoubleQuotes(string input, string result)
        {
            Assert.Equal(result, input.AsSpan().RemoveLiteralsAndQuotes());
        }
    }
}
