using System;
using System.Linq;
using System.Threading;

using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Attributes;

using Moq;

using Xunit;

namespace MatthiWare.CommandLine.Tests
{
    public class CommandLineParserTests
    {
        public class MyCommand : Command<object, object>
        {
            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                builder.Name("my");
            }

            public override void OnConfigure(ICommandConfigurationBuilder<object> builder)
            {
                builder.Name("my");
            }
        }

        [Fact]
        public void CommandLineParserUsesCorrectOptions()
        {
            var opt = new CommandLineParserOptions();

            var parser = new CommandLineParser(opt);

            Assert.Equal(opt, parser.ParserOptions);
        }

        [Fact]
        public void CommandLineParserUsesContainerCorrectly()
        {
            var commandMock = new Mock<MyCommand>();
            commandMock.Setup(
                c => c.OnConfigure(It.IsAny<ICommandConfigurationBuilder<object>>()))
                .CallBase().Verifiable("OnConfigure not called");

            commandMock.Setup(c => c.OnExecute(It.IsAny<object>(), It.IsAny<object>())).Verifiable("OnExecute not called");

            var containerMock = new Mock<IContainerResolver>();
            containerMock.Setup(c => c.Resolve<MyCommand>()).Returns(commandMock.Object).Verifiable();

            var parser = new CommandLineParser<object>(containerMock.Object);

            parser.RegisterCommand<MyCommand, object>();

            var result = parser.Parse(new[] { "app.exe", "my" });

            result.AssertNoErrors();

            commandMock.VerifyAll();
            containerMock.VerifyAll();
        }

        [Fact]
        public void CommandLinerParserPassesContainerCorreclty()
        {
            var containerResolver = new Mock<IContainerResolver>();
            var options = new CommandLineParserOptions();
            var parser = new CommandLineParser(containerResolver.Object);

            Assert.Equal(containerResolver.Object, parser.ContainerResolver);

            parser = new CommandLineParser(options, containerResolver.Object);

            Assert.Equal(containerResolver.Object, parser.ContainerResolver);
            Assert.Equal(options, parser.ParserOptions);
        }

        [Fact]
        public void CommandLinerParserPassesResolverCorreclty()
        {
            var resolverMock = new Mock<IArgumentResolverFactory>();
            var options = new CommandLineParserOptions();
            var parser = new CommandLineParser(resolverMock.Object);

            Assert.Equal(resolverMock.Object, parser.ArgumentResolverFactory);

            parser = new CommandLineParser(options, resolverMock.Object);

            Assert.Equal(resolverMock.Object, parser.ArgumentResolverFactory);
            Assert.Equal(options, parser.ParserOptions);
        }

        [Fact]
        public void CommandLinerParserPassesResolverAndContainerCorreclty()
        {
            var resolverMock = new Mock<IArgumentResolverFactory>();

            var containerMock = new Mock<IContainerResolver>();

            var options = new CommandLineParserOptions();

            var parser = new CommandLineParser(options, resolverMock.Object, containerMock.Object);

            Assert.Equal(resolverMock.Object, parser.ArgumentResolverFactory);
            Assert.Equal(containerMock.Object, parser.ContainerResolver);
            Assert.Equal(options, parser.ParserOptions);
        }

        [Fact]
        public void AutoExecuteCommandsWithExceptionDoesntCrashTheParser()
        {
            var parser = new CommandLineParser();

            var ex = new Exception("uh-oh");

            parser.AddCommand()
                .Name("test")
                .InvokeCommand(true)
                .Required(true)
                .OnExecuting(_ => throw ex);

            var result = parser.Parse(new[] { "test" });

            Assert.True(result.HasErrors);

            Assert.Equal(ex, result.Errors.First());
        }

        [Fact]
        public void CommandLineParserUsesArgumentFactoryCorrectly()
        {
            var resolverMock = new Mock<ArgumentResolver<string>>();
            resolverMock.Setup(_ => _.CanResolve(It.IsAny<ArgumentModel>())).Returns(true).Verifiable();
            resolverMock.Setup(_ => _.Resolve(It.IsAny<ArgumentModel>())).Returns("return").Verifiable();

            var argResolverFactory = new Mock<IArgumentResolverFactory>();
            argResolverFactory.Setup(c => c.Contains(typeof(string))).Returns(true);
            argResolverFactory.Setup(c => c.CreateResolver(typeof(string))).Returns(resolverMock.Object).Verifiable();

            var parser = new CommandLineParser<AddOption>(argResolverFactory.Object);

            parser.Configure(p => p.Message).Name("m");

            var result = parser.Parse(new[] { "app.exe", "-m" });

            result.AssertNoErrors();

            resolverMock.VerifyAll();
            argResolverFactory.Verify();
        }

        [Fact]
        public void ParseTests()
        {
            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Option1)
                .Name("o")
                .Default("Default message")
                .Required();

            var parsed = parser.Parse(new string[] { "app.exe", "-o", "test" });

            Assert.NotNull(parsed);

            parsed.AssertNoErrors();

            Assert.Equal("test", parsed.Result.Option1);
        }

        [Theory]
        [InlineData(new[] { "app.exe", "-e", "Opt1" }, false, EnumOption.Opt1)]
        [InlineData(new[] { "app.exe", "-e", "opt1" }, false, EnumOption.Opt1)]
        [InlineData(new[] { "app.exe", "-e", "Opt2" }, false, EnumOption.Opt2)]
        [InlineData(new[] { "app.exe", "-e", "bla" }, true, default(EnumOption))]
        [InlineData(new[] { "app.exe", "-e" }, true, default(EnumOption))]
        public void ParseEnumInArguments(string[] args, bool hasErrors, EnumOption enumOption)
        {
            var parser = new CommandLineParser<EnumOptions>();

            parser.Configure(opt => opt.EnumOption)
                .Name("e")
                .Required();

            var result = parser.Parse(args);

            Assert.Equal(hasErrors, result.HasErrors);

            Assert.Equal(enumOption, result.Result.EnumOption);
        }

        [Theory]
        [InlineData(new[] { "app.exe", "-1", "message1", "-2", "-3" }, "message1", "message2", "message3")]
        [InlineData(new[] { "app.exe", "-1", "-2", "message2", "-3" }, "message1", "message2", "message3")]
        [InlineData(new[] { "app.exe", "-1", "-2", "-3" }, "message1", "message2", "message3")]
        [InlineData(new[] { "-1", "message1", "-2", "-3" }, "message1", "message2", "message3")]
        [InlineData(new[] { "-1", "-2", "message2", "-3" }, "message1", "message2", "message3")]
        [InlineData(new[] { "-1", "-2", "-3" }, "message1", "message2", "message3")]
        public void ParseWithDefaults(string[] args, string result1, string result2, string result3)
        {
            var parser = new CommandLineParser<OptionsWithThreeParams>();

            parser.Configure(opt => opt.Option1)
                .Name("1")
                .Default(result1)
                .Required();

            parser.Configure(opt => opt.Option2)
                .Name("2")
                .Default(result2)
                .Required();

            parser.Configure(opt => opt.Option3)
                .Name("3")
                .Default(result3)
                .Required();

            var parsed = parser.Parse(args);

            parsed.AssertNoErrors();

            Assert.Equal(result1, parsed.Result.Option1);
            Assert.Equal(result2, parsed.Result.Option2);
            Assert.Equal(result3, parsed.Result.Option3);
        }

        [Fact]
        public void ParseWithCustomParserInAttributeConfiguredModelTests()
        {
            var resolver = new Mock<ArgumentResolver<object>>();

            var obj = new object();

            resolver.Setup(_ => _.CanResolve(It.IsAny<ArgumentModel>())).Returns(true);
            resolver.Setup(_ => _.Resolve(It.IsAny<ArgumentModel>())).Returns(obj);

            var parser = new CommandLineParser<ObjOption>();
            parser.ArgumentResolverFactory.Register(resolver.Object);

            var result = parser.Parse(new[] { "app.exe", "-p", "sample" });

            result.AssertNoErrors();

            Assert.Same(obj, result.Result.Param);
        }

        [Fact]
        public void ParseWithCommandTests()
        {
            var wait = new ManualResetEvent(false);

            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Option1)
                .Name("o")
                .Default("Default message")
                .Required();

            var addCmd = parser.AddCommand<AddOption>()
                .Name("add")
                .OnExecuting((opt, cmdOpt) =>
                {
                    Assert.Equal("test", opt.Option1);
                    Assert.Equal("my message", cmdOpt.Message);
                    wait.Set();
                });

            addCmd.Configure(opt => opt.Message)
                .Name("m", "message")
                .Required();

            var parsed = parser.Parse(new string[] { "app.exe", "-o", "test", "add", "-m", "my message" });

            parsed.AssertNoErrors();

            Assert.Equal("test", parsed.Result.Option1);

            parsed.ExecuteCommands();

            Assert.True(wait.WaitOne(2000));
        }

        [Theory]
        [InlineData(new[] { "app.exe", "add", "-m", "message2", "-m", "message1" }, "message1", "message2")]
        [InlineData(new[] { "app.exe", "-m", "message1", "add", "-m", "message2" }, "message1", "message2")]
        [InlineData(new[] { "add", "-m", "message2", "-m", "message1" }, "message1", "message2")]
        [InlineData(new[] { "-m", "message1", "add", "-m", "message2" }, "message1", "message2")]
        public void ParseCommandTests(string[] args, string result1, string result2)
        {
            var parser = new CommandLineParser<AddOption>();
            var wait = new ManualResetEvent(false);

            parser.AddCommand<AddOption>()
                .Name("add")
                .Required()
                .OnExecuting((opt1, opt2) =>
                {
                    wait.Set();

                    Assert.Equal(result2, opt2.Message);
                })
                .Configure(c => c.Message)
                    .Name("m", "message")
                    .Required();

            parser.Configure(opt => opt.Message)
                .Name("m", "message")
                .Required();

            var result = parser.Parse(args);

            result.AssertNoErrors();

            Assert.Equal(result1, result.Result.Message);

            Assert.True(wait.WaitOne(2000));
        }

        [Theory]
        [InlineData(new string[] { "-x", "" }, true)]
        [InlineData(new string[] { "-x" }, true)]
        [InlineData(new string[] { "-x", "1" }, true)]
        [InlineData(new string[] { "-x", "true" }, true)]
        [InlineData(new string[] { "-x", "false" }, false)]
        public void BoolResolverSpecialCaseParsesCorrectly(string[] args, bool expected)
        {
            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Option2)
                .Name("x", "xsomething")
                .Required();

            var result = parser.Parse(args);

            result.AssertNoErrors();

            Assert.Equal(expected, result.Result.Option2);
        }

        #region Issue_35_Bool_Option_Not_Parsed_Correctly

        [Theory]
        [InlineData(new string[] { "-v", "command" }, true)]
        [InlineData(new string[] { "command", "-v" }, true)]
        public void BoolResolverSpecialCaseParsesCorrectlyWithDefaultValueAndNotBeingSpecified(string[] args, bool expected)
        {
            var parser = new CommandLineParser<Model_Issue_35>();

            parser.AddCommand().Name("command");

            var result = parser.Parse(args);

            result.AssertNoErrors();

            Assert.Equal(expected, result.Result.VerbA);
        }

        private class Model_Issue_35
        {
            [Name("v", "verb"), Description("Print usage"), DefaultValue(false)]
            public bool VerbA { get; set; }
        }

        #endregion

        [Fact]
        public void ConfigureTests()
        {
            var parser = new CommandLineParser<Options>();

            parser.Configure(opt => opt.Option1)
                .Name("o", "opt")
                .Default("Default message")
                .Required();

            parser.Configure(opt => opt.Option2)
                .Name("x", "xsomething")
                .Required();

            Assert.Equal(2, parser.Options.Count);

            var message = parser.Options[0];
            var option = parser.Options[1];

            Assert.NotNull(message);
            Assert.NotNull(option);

            Assert.Equal("-o", message.ShortName);
            Assert.Equal("--opt", message.LongName);
            Assert.True(message.HasDefault);

            Assert.Equal("-x", option.ShortName);
            Assert.Equal("--xsomething", option.LongName);
            Assert.False(option.HasDefault);
        }

        private class ObjOption
        {
            [Name("p"), Required]
            public object Param { get; set; }
        }

        private class AddOption
        {
            public string Message { get; set; }
        }

        private class Options
        {
            public string Option1 { get; set; }
            public bool Option2 { get; set; }
        }

        public enum EnumOption
        {
            Opt1,
            Opt2
        }

        public class EnumOptions
        {
            public EnumOption EnumOption { get; set; }
        }

        private class OptionsWithThreeParams
        {
            public string Option1 { get; set; }
            public string Option2 { get; set; }
            public string Option3 { get; set; }
        }
    }
}
