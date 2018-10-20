using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using MatthiWare.CommandLine.Core.Parsing;
using Moq;
using Xunit;

namespace MatthiWare.CommandLineParser.Tests.Parsing
{
    public class ParserResultTest
    {
        [Fact]
        public void TestMergeResultOfErrorsWorks()
        {
            var result = new ParseResult<object>();

            Assert.Null(result.Error);

            var exception1 = new Exception("test");

            result.MergeResult(new[] { exception1 });

            Assert.True(result.HasErrors);
            Assert.Same(exception1, result.Error);

            result.MergeResult(new[] { new Exception("2") });

            Assert.True(result.HasErrors);
            Assert.NotSame(exception1, result.Error);

            Assert.IsType<AggregateException>(result.Error);
        }

        [Fact]
        public void TestMergeResultOfCommandResultWorks()
        {
            var result = new ParseResult<object>();

            var mockCmdResult = new Mock<ICommandParserResult>();

            mockCmdResult.SetupGet(x => x.HasErrors).Returns(false);
            mockCmdResult.SetupGet(x => x.Error).Returns((Exception)null);

            result.MergeResult(mockCmdResult.Object);

            mockCmdResult.VerifyGet(x => x.HasErrors);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void TestMergeResultOfResultWorks()
        {
            var result = new ParseResult<object>();

            var obj = new object();

            result.MergeResult(obj);

            Assert.Null(result.Error);

            Assert.Same(obj, result.Result);
        }
    }
}
