using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Core.Parsing;
using Xunit;

namespace MatthiWare.CommandLineParser.Tests.Parsing
{
    public class ParserResultTest
    {


        [Fact]
        public void ParserResultFromErrorContainsErrorAndNoResult()
        {
            var exception = new Exception("dummy exception");

            var result = ParseResult<object>.FromError(exception);

            Assert.Null(result.Result);
            Assert.NotNull(result.Error);
            Assert.True(result.HasErrors);

            Assert.Equal(exception, result.Error);
        }

        [Fact]
        public void ParserResultFromResultContainsResultAndNoErrors()
        {
            var obj = new object();

            var result = ParseResult<object>.FromResult(obj);

            Assert.NotNull(result.Result);
            Assert.Null(result.Error);
            Assert.False(result.HasErrors);

            Assert.Equal(obj, result.Result);
        }

    }
}
