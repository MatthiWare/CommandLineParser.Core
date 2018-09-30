using System;
using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core;
using Moq;
using Xunit;

namespace MatthiWare.CommandLineParser.Tests
{
    public class ExtensionsTests
    {
        //[Fact]
        //public void TestV1()
        //{
        //    var testString = "this is my test string \"with some quotes\" the end. '\"Here is some literal\"' ";

        //    var resultArr = new string[] { "this", "is", "my", "test", "string", "with some quotes", "the", "end.", "\"Here is some literal\"", " " };

        //    int i = 0;
        //    foreach (var token in Extensions.SplitOnWhitespace(testString))
        //    {
        //        Assert.Equal(resultArr[i++], token);
        //    }

        //    Assert.Equal(resultArr.Length, i);
        //}

        //[Theory]
        //[InlineData("\"with some quotes\"", "with some quotes")]
        //[InlineData("'\"with some quotes\"'", "\"with some quotes\"")]
        //[InlineData("test", "test")]
        //public void TestRemoveLiteralAndDoubleQuotes(string input, string result)
        //{
        //    Assert.Equal(result, input.AsSpan().RemoveLiteralsAndQuotes());
        //}
    }
}
