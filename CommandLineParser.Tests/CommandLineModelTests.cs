using MatthiWare.CommandLine.Core.Attributes;

using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests
{
    public class CommandLineModelTests : TestBase
    {
        public CommandLineModelTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void TestBasicModel()
        {
            var parser = new CommandLineParser<Model>(Services);

            Assert.Equal(1, parser.Options.Count);

            var message = parser.Options[0];

            Assert.NotNull(message);

            Assert.True(message.HasLongName && message.HasShortName);

            Assert.Equal("-m", message.ShortName);
            Assert.Equal("--message", message.LongName);

            Assert.True(message.HasDefault);
            Assert.True(message.IsRequired);

            Assert.Equal("Help", message.Description);
        }

        [Fact]
        public void TestBasicModelWithOverwritingUsingFluentApi()
        {
            var parser = new CommandLineParser<Model>(Services);

            parser.Configure(_ => _.Message)
                .Required(false)
                .Description("Different");

            Assert.Equal(1, parser.Options.Count);

            var message = parser.Options[0];

            Assert.NotNull(message);

            Assert.True(message.HasLongName && message.HasShortName);

            Assert.Equal("-m", message.ShortName);
            Assert.Equal("--message", message.LongName);

            Assert.True(message.HasDefault);
            Assert.False(message.IsRequired);

            Assert.Equal("Different", message.Description);
        }

        private class Model
        {
            [Required, Name("m", "message"), DefaultValue("not found"), Description("Help")]
            public string Message { get; set; }
        }
    }
}
