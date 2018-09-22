using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions;
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
            var optionMock = new Mock<ICommandLineArgumentOption<string>>();
            var builder = new OptionBuilder<object, string>(new object(), optionMock.Object, obj => obj.ToString());

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

            optionMock.VerifySet(opt => opt.ShortName = sShort, Times.Once);
            optionMock.VerifySet(opt => opt.LongName = sLong, Times.Once);
            optionMock.VerifySet(opt => opt.DefaultValue = sDefault, Times.Once);
            optionMock.VerifySet(opt => opt.HelpText = sHelp, Times.Once);
            optionMock.VerifySet(opt => opt.IsRequired = true, Times.Once);
        }

    }
}
