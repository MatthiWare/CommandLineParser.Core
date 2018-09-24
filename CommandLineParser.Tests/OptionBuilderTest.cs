using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core;
using Moq;
using Xunit;

namespace CommandLineParser.Tests
{
    public class OptionBuilderTest
    {

        [Fact]
        public void OptionBuilderConfiguresOptionCorrectly()
        {
            var resolverMock = new Mock<ICommandLineArgumentResolver<string>>();
            var option = new CommandLineArgumentOption<object, string>(new object(), o => o.ToString(), resolverMock.Object);
            var builder = option as IOptionBuilder<string>;

            string sDefault = "default";
            string sHelp = "help";
            string sLong = "long";
            string sShort = "short";

            builder
                .Default(sDefault)
                .HelpText(sHelp)
                .LongName(sLong)
                .ShortName(sShort)
                .Required();

            Assert.True(option.HasDefault);
            Assert.Equal(sDefault, option.DefaultValue);

            Assert.True(option.HasLongName);
            Assert.Equal(sLong, option.LongName);

            Assert.Equal(sHelp, option.HelpText);

            Assert.True(option.HasShortName);
            Assert.Equal(sShort, option.ShortName);

        }
    }
}