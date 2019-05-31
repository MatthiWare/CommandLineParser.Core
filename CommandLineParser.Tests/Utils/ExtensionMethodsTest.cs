using Xunit;
using MatthiWare.CommandLine.Core.Utils;
using System.Linq;
using Moq;
using MatthiWare.CommandLine.Abstractions;

namespace MatthiWare.CommandLine.Tests.Utils
{
    public class ExtensionMethodsTest
    {
        [Fact]
        public void TestSplitOnPostFixWorksCorrectly()
        {
            var settings = new CommandLineParserOptions();

            var mockTestOption1 = CreateMock(settings, "test").Object;
            var mockTestOption2 = CreateMock(settings, "test2", "test2").Object;
            var mockTestOption3 = CreateMock(settings, "test3").Object;
            var mockTestOption4 = CreateMock(settings, "blabla", "blabla").Object;

            var options = new[] 
            {
                mockTestOption1,
                mockTestOption2,
                mockTestOption3,
                mockTestOption4
            };

            var input = new[] { "--test=1", "-test2=\"some value\"", "--test3=some value", "-blabla", "third" };
            var output = input.SplitOnPostfix(settings, options).ToArray();

            Assert.Equal("--test", output[0]);
            Assert.Equal("1", output[1]);
            Assert.Equal("-test2", output[2]);
            Assert.Equal("\"some value\"", output[3]);
            Assert.Equal("--test3", output[4]);
            Assert.Equal("some value", output[5]);
            Assert.Equal("-blabla", output[6]);
            Assert.Equal("third", output[7]);
        }

        [Fact]
        public void TestSplitOnPostFixWorksCorrectly_2()
        {
            var settings = new CommandLineParserOptions();

            var testOption = CreateMock(settings, "test").Object;

            var options = new[]
            {
                testOption,
            };

            var input = new[] { "--test", "some=value" };

            var output = input.SplitOnPostfix(settings, options).ToArray();

            Assert.Equal("--test", output[0]);
            Assert.Equal("some=value", output[1]);
        }

        private Mock<ICommandLineOption> CreateMock(CommandLineParserOptions settings, string longName, string shortName = "")
        {
            var mock = new Mock<ICommandLineOption>();

            mock.SetupGet(option => option.LongName).Returns($"{settings.PrefixLongOption}{longName}");
            mock.SetupGet(option => option.HasLongName).Returns(true);

            if (!string.IsNullOrWhiteSpace(shortName))
            {
                mock.SetupGet(option => option.ShortName).Returns($"{settings.PrefixShortOption}{shortName}");
                mock.SetupGet(option => option.HasShortName).Returns(true);
            }

            return mock;
        }
    }
}
