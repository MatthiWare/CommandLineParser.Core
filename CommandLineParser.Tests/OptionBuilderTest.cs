using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;
using Microsoft.Extensions.Logging.Abstractions;
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
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(_ => _.GetService(It.IsAny<Type>())).Returns(resolverMock.Object);

            var cmdOption = new CommandLineOption<object>(
                new CommandLineParserOptions { PrefixLongOption = string.Empty, PrefixShortOption = string.Empty },
                new object(),
                XUnitExtensions.CreateLambda<object, string>(o => o.ToString()),
                new DefaultResolver<object>(NullLogger<CommandLineParser>.Instance), NullLogger.Instance);

            var builder = cmdOption as IOptionBuilder;
            var option = cmdOption as CommandLineOptionBase;

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