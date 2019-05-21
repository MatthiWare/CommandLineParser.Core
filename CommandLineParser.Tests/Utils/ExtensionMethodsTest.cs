using Xunit;
using MatthiWare.CommandLine.Core.Utils;
using System.Linq;

namespace MatthiWare.CommandLine.Tests.Utils
{
    public class ExtensionMethodsTest
    {

        [Fact]
        public void TestSplitOnPostFixWorksCorrectly()
        {
            var input = new[] { "--test=1", "--test2=\"some value\"", "--blabla", "third" };

            var output = input.SplitOnPostfix(true, true, "=", "=").ToArray();

            Assert.Equal("--test", output[0]);
            Assert.Equal("1", output[1]);
            Assert.Equal("--test2", output[2]);
            Assert.Equal("\"some value\"", output[3]);
            Assert.Equal("--blabla", output[4]);
            Assert.Equal("third", output[5]);
        }

    }
}
