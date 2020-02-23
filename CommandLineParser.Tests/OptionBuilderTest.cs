using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core;
using Moq;
using System;
using Xunit;

namespace MatthiWare.CommandLine.Tests
{
    public class OptionBuilderTest
    {
        [Fact]
        public void OptionBuilderConfiguresOptionCorrectly()
        {
            var resolverMock = new Mock<ICommandLineArgumentResolver<string>>();
            var resolverFactoryMock = new Mock<IArgumentResolverFactory>();
            resolverFactoryMock.Setup(_ => _.CreateResolver(It.IsAny<Type>())).Returns(resolverMock.Object);

            var option = new CommandLineOption(
                new CommandLineParserOptions { PrefixLongOption = string.Empty, PrefixShortOption = string.Empty },
                new object(),
                XUnitExtensions.CreateLambda<object, string>(o => o.ToString()),
                resolverFactoryMock.Object);

            var builder = option as IOptionBuilder;

            string sDefault = "default";
            string sHelp = "help";
            string sLong = "long";
            string sShort = "short";

            builder
                .Default(sDefault)
                .Description(sHelp)
                .Name(sShort, sLong)
                .Required();

            Assert.True(option.HasDefault);
            Assert.Equal(sDefault, option.DefaultValue);

            Assert.True(option.HasLongName);
            Assert.Equal(sLong, option.LongName);

            Assert.Equal(sHelp, option.Description);

            Assert.True(option.HasShortName);
            Assert.Equal(sShort, option.ShortName);
        }
    }
}