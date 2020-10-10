using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Core.Usage;
using Moq;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Usage
{
    public class DamerauLevenshteinTests : TestBase
    {
        public DamerauLevenshteinTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData(3, "CA", "ABC")]
        [InlineData(1, "Test", "Tst")]
        [InlineData(0, "Test", "Test")]
        [InlineData(4, "", "Test")]
        [InlineData(4, "Test", "")]
        public void DistanceOfStrings(int expected, string a, string b)
        {
            var dl = new DamerauLevenshteinSuggestionProvider();

            Assert.Equal(expected, dl.FindDistance(a, b));
        }

        [Fact]
        public void SuggestionsAreGatheredFromAllAvailablePlaces()
        {
            var containerMock = new Mock<ICommandLineCommandContainer>();
            var cmdMock = new Mock<ICommandLineCommand>();
            var optionMock = new Mock<ICommandLineOption>();

            cmdMock.SetupGet(_ => _.Name).Returns("test1");
            optionMock.SetupGet(_ => _.HasLongName).Returns(true);
            optionMock.SetupGet(_ => _.HasShortName).Returns(true);
            optionMock.SetupGet(_ => _.LongName).Returns("test2");
            optionMock.SetupGet(_ => _.ShortName).Returns("test3");

            containerMock.SetupGet(_ => _.Commands).Returns(AsList(cmdMock.Object));
            containerMock.SetupGet(_ => _.Options).Returns(AsList(optionMock.Object));

            var dl = new DamerauLevenshteinSuggestionProvider();

            var result = dl.GetSuggestions("test", containerMock.Object);

            Assert.Contains("test1", result);
            Assert.Contains("test2", result);
            Assert.Contains("test3", result);

            List<T> AsList<T>(T obj)
            {
                var list = new List<T>(1) { obj };
                return list;
            }
        }

        [Fact]
        public void InvalidSuggestionIsNotReturned()
        {
            var containerMock = new Mock<ICommandLineCommandContainer>();
            var cmdMock = new Mock<ICommandLineCommand>();
            var optionMock = new Mock<ICommandLineOption>();

            cmdMock.SetupGet(_ => _.Name).Returns("test");

            containerMock.SetupGet(_ => _.Commands).Returns(AsList(cmdMock.Object));
            containerMock.SetupGet(_ => _.Options).Returns(new List<ICommandLineOption>());

            var dl = new DamerauLevenshteinSuggestionProvider();

            var result = dl.GetSuggestions("abc", containerMock.Object);

            Assert.Empty(result);

            List<T> AsList<T>(T obj)
            {
                var list = new List<T>(1) { obj };
                return list;
            }
        }

        [Fact]
        public void NoSuggestionsReturnsEmpty()
        {
            var containerMock = new Mock<ICommandLineCommandContainer>();

            containerMock.SetupGet(_ => _.Commands).Returns(new List<ICommandLineCommand>());
            containerMock.SetupGet(_ => _.Options).Returns(new List<ICommandLineOption>());

            var dl = new DamerauLevenshteinSuggestionProvider();

            var result = dl.GetSuggestions("abc", containerMock.Object);

            Assert.Empty(result);
        }

        [Fact]
        public void Test()
        {
            var parser = new CommandLineParser(Services);

            parser.AddCommand().Name("cmd");

            var result = parser.Parse(new[] { "app.exe", "cmdd" });

            result.AssertNoErrors();
        }
    }
}
