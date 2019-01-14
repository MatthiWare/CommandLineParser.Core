using System;
using System.Collections.Generic;
using System.Linq;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using MatthiWare.CommandLine.Core.Parsing;

using Moq;

using Xunit;

namespace MatthiWare.CommandLine.Tests.Parsing
{
    public class ParserResultTest
    {
        [Fact]
        public void TestMergeResultOfErrorsWorks()
        {
            var result = new ParseResult<object>();

            Assert.Empty(result.Errors);

            var exception1 = new Exception("test");

            result.MergeResult(new[] { exception1 });

            Assert.True(result.HasErrors);
            Assert.Same(exception1, result.Errors.First());

            result.MergeResult(new[] { new Exception("2") });

            Assert.True(result.HasErrors);
            Assert.NotSame(exception1, result.Errors.Skip(1).First());
        }

        [Fact]
        public void TestMergeResultOfCommandResultWorks()
        {
            var result = new ParseResult<object>();

            var mockCmdResult = new Mock<ICommandParserResult>();

            mockCmdResult.SetupGet(x => x.HasErrors).Returns(false);
            mockCmdResult.SetupGet(x => x.Errors).Returns(new List<Exception>());

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

            Assert.False(result.HasErrors);

            Assert.Empty(result.Errors);

            Assert.Same(obj, result.Result);
        }
    }
}
