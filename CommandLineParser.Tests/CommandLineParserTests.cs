using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests
{
    public class CommandLineParserTests : TestBase
    {
        public CommandLineParserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

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
        public void OrderAttributeWorks()
        {
            var from = @"path/from/file";
            var to = @"path/to/file";

            var parser = new CommandLineParser<OrderModel>(Services);

            var result = parser.Parse(new string[] { "app.exe", from, to });

            result.AssertNoErrors();

            Assert.Equal(from, result.Result.From);
            Assert.Equal(to, result.Result.To);
        }

        [Fact]
        public void OrderAttributeWorks2()
        {
            var from = @"path/from/file";
            var to = @"path/to/file";

            var parser = new CommandLineParser<OrderModel>(Services);

            var result = parser.Parse(new string[] { "app.exe", "-r", "5", from, to });

            result.AssertNoErrors();

            Assert.Equal(from, result.Result.From);
            Assert.Equal(to, result.Result.To);
        }

        [Fact]
        public void OrderAttributeInCommandWorks()
        {
            var from = @"path/from/file";
            var to = @"path/to/file";

            var parser = new CommandLineParser(Services);

            parser.AddCommand<OrderModel>().Name("cmd").Required().OnExecuting((o, model) => 
            {
                Assert.Equal(from, model.From);
                Assert.Equal(to, model.To);
            });

            var result = parser.Parse(new string[] { "app.exe", "cmd", from, to });

            result.AssertNoErrors();
        }

        [Fact]
        public void OrderedOptions_With_Named_Option_Between_Does_Not_work()
        {
            var from = @"path/from/file";
            var to = @"path/to/file";

            var parser = new CommandLineParser<OrderModel>(Services);

            var result = parser.Parse(new string[] { "app.exe", from, "-r", "5", to });

            Assert.True(result.HasErrors);

            Assert.Equal(from, result.Result.From);
            Assert.NotEqual(to, result.Result.To);
        }

        [Fact]
        public void OrderedOptions_With_Named_Option_Between_Does_Not_work2()
        {
            var from = 5;
            var to = 10;

            var parser = new CommandLineParser<OrderModelInt>(Services);

            var result = parser.Parse(new string[] { "app.exe", from.ToString(), "oops", to.ToString() });

            Assert.True(result.HasErrors);

            Assert.Equal(from, result.Result.From);
            Assert.NotEqual(to, result.Result.To);
        }

        [Fact]
        public void StopProcessingWorks()
        {
            var options = new CommandLineParserOptions 
            {
                StopParsingAfter = "--"
            };

            var parser = new CommandLineParser<OrderModelInt>(options, Services);

            var result = parser.Parse(new string[] { "app.exe", "10", "20", "--", "some random stuff", "nothing to see here", "yadi yadi yadi", "-r", "10" });

            result.AssertNoErrors();

            Assert.Equal(10, result.Result.From);
            Assert.Equal(20, result.Result.To);
            Assert.NotEqual(10, result.Result.Random);
        }

        [Theory]
        [InlineData("", "--")]
        [InlineData("-", "")]
        [InlineData("---", "---")]
        public void InvalidOptionsThrowException(string shortOption, string longOption)
        {
            var options = new CommandLineParserOptions
            {
                PrefixShortOption = shortOption,
                PrefixLongOption = longOption
            };

            Assert.Throws<ArgumentException>(() => new CommandLineParser(options, Services));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CommandLineParserParsesCorrectOptionsWithPostfix(bool useShort)
        {
            var query = $"{(useShort ? "-" : "--")}p=some text";

            var parser = new CommandLineParser<AddOption>();

            parser.Configure(p => p.Message).Name("p", "p").Required();

            var result = parser.Parse(new string[] { query });

            result.AssertNoErrors();

            Assert.Equal("some text", result.Result.Message);
        }

        [Fact]
        public void CommandLineParserUsesCorrectOptions()
        {
            var opt = new CommandLineParserOptions();

            var parser = new CommandLineParser(opt, Services);

            Assert.Equal(opt, parser.ParserOptions);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CommandLineParserUsesContainerCorrectly(bool generic)
        {
            var commandMock = new Mock<MyCommand>();
            commandMock
                .Setup(c => c.OnConfigure(It.IsAny<ICommandConfigurationBuilder<object>>()))
                .CallBase()
                .Verifiable("OnConfigure not called");

            commandMock
                .Setup(c => c.OnExecute(It.IsAny<object>(), It.IsAny<object>()))
                .Verifiable("OnExecute not called");

            Services.AddSingleton(commandMock.Object);

            var parser = new CommandLineParser<object>(Services);

            if (generic)
            {
                parser.RegisterCommand<MyCommand, object>();
            }
            else
            {
                parser.RegisterCommand(typeof(MyCommand), typeof(object));
            }

            var result = parser.Parse(new[] { "app.exe", "my" });

            result.AssertNoErrors();

            commandMock.VerifyAll();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CommandLineParserUsesContainerCorrectlyAsync(bool generic)
        {
            var commandMock = new Mock<MyCommand>();
            commandMock
                .Setup(c => c.OnConfigure(It.IsAny<ICommandConfigurationBuilder<object>>()))
                .CallBase()
                .Verifiable("OnConfigure not called");

            commandMock
                .Setup(c => c.OnExecuteAsync(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Verifiable("OnExecute not called");

            Services.AddSingleton(commandMock.Object);

            var parser = new CommandLineParser<object>(Services);

            if (generic)
            {
                parser.RegisterCommand<MyCommand, object>();
            }
            else
            {
                parser.RegisterCommand(typeof(MyCommand), typeof(object));
            }

            var result = await parser.ParseAsync(new[] { "app.exe", "my" });

            result.AssertNoErrors();

            commandMock.VerifyAll();
        }

        [Fact]
        public void AutoExecuteCommandsWithExceptionDoesntCrashTheParser()
        {
            var parser = new CommandLineParser(Services);

            var ex = new Exception("uh-oh");

            parser.AddCommand()
                .Name("test")
                .InvokeCommand(true)
                .Required(true)
                .OnExecuting(_ => throw ex);

            var result = parser.Parse(new[] { "test" });

            Assert.True(result.HasErrors);

            Assert.Equal(ex, result.Errors.First().GetBaseException());
        }

        [Fact]
        public async Task AutoExecuteCommandsWithExceptionDoesntCrashTheParserAsync()
        {
            var parser = new CommandLineParser(Services);

            var ex = new Exception("uh-oh");

            parser.AddCommand()
                .Name("test")
                .InvokeCommand(true)
                .Required(true)
                .OnExecutingAsync(async (_, __) =>
                {
                    await Task.Delay(1);
                    throw ex;
                });

            var result = await parser.ParseAsync(new[] { "test" });

            Assert.True(result.HasErrors);

            Assert.Equal(ex, result.Errors.First().GetBaseException());
        }

        [Fact]
        public void ParseTests()
        {
            var parser = new CommandLineParser<Options>(Services);

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
        [InlineData(new[] { "app.exe", "-e=opt1" }, false, EnumOption.Opt1)]
        [InlineData(new[] { "app.exe", "-e", "Opt2" }, false, EnumOption.Opt2)]
        [InlineData(new[] { "app.exe", "-e", "bla" }, true, default(EnumOption))]
        [InlineData(new[] { "app.exe", "-e" }, true, default(EnumOption))]
        public void ParseEnumInArguments(string[] args, bool hasErrors, EnumOption enumOption)
        {
            var parser = new CommandLineParser<EnumOptions>(Services);

            parser.Configure(opt => opt.EnumOption)
                .Name("e")
                .Required();

            var result = parser.Parse(args);

            Assert.Equal(hasErrors, result.HasErrors);

            Assert.Equal(enumOption, result.Result.EnumOption);
        }

        [Theory]
        // string
        [InlineData(typeof(string), new[] { "-1=message1", "-2", "-3" }, "default", "message1", "default", "default")]
        [InlineData(typeof(string), new[] { "-1", "-2", "message2", "-3" }, "default", "default", "message2", "default")]
        [InlineData(typeof(string), new[] { "-1", "-2", "-3" }, "default", "default", "default", "default")]
        // bool
        [InlineData(typeof(bool), new[] { "-1=false", "-2", "-3" }, false, false, true, true)]
        [InlineData(typeof(bool), new[] { "-1", "-2", "false", "-3" }, false, true, false, true)]
        [InlineData(typeof(bool), new[] { "-1", "-2", "-3" }, false, true, true, true)]
        //// int
        [InlineData(typeof(int), new[] { "-1", "5", "-2", "-3" }, 0, 5, 0, 0)]
        [InlineData(typeof(int), new[] { "-1", "-2", "5", "-3" }, 0, 0, 5, 0)]
        [InlineData(typeof(int), new[] { "-1", "-2", "-3" }, 0, 0, 0, 0)]
        // double
        [InlineData(typeof(double), new[] { "-1", "0.5", "-2", "-3" }, 0.1, 0.5, 0.1, 0.1)]
        [InlineData(typeof(double), new[] { "-1", "-2", "0.5", "-3" }, 0.1, 0.1, 0.5, 0.1)]
        [InlineData(typeof(double), new[] { "-1", "-2", "-3" }, 0.1, 0.1, 0.1, 0.1)]
        //// enum
        [InlineData(typeof(EnumOption), new[] { "-1", "Opt1", "-2", "-3" }, EnumOption.Opt1, EnumOption.Opt1, "message2", "message3")]
        [InlineData(typeof(EnumOption), new[] { "-1", "-2", "Opt1", "-3" }, EnumOption.Opt1, "message1", EnumOption.Opt1, "message3")]
        [InlineData(typeof(EnumOption), new[] { "-1", "-2", "-3" }, EnumOption.Opt1, "message1", "message2", "message3")]
        public void ParseWithDefaults(Type type, string[] args, object defaultValue, object result1, object result2, object result3)
        {
            if (type == typeof(bool))
            {
                Test<bool>();
            }
            else if (type == typeof(string))
            {
                Test<string>();
            }
            else if (type == typeof(int))
            {
                Test<int>();
            }
            else if (type == typeof(double))
            {
                Test<double>();
            }
            else if (type == typeof(Enum))
            {
                Test<Enum>();
            }

            void Test<T>()
            {
                TestParsingWithDefaults<T>(args, (T)defaultValue, (T)result1, (T)result2, (T)result3);
            }
        }

        private void TestParsingWithDefaults<T>(string[] args, T defaultValue, T result1, T result2, T result3)
        {
            var parser = new CommandLineParser<OptionsWithThreeParams<T>>(Services);

            parser.Configure(opt => opt.Option1)
                .Name("1")
                .Default(defaultValue)
                .Required();

            parser.Configure(opt => opt.Option2)
                .Name("2")
                .Default(defaultValue)
                .Required();

            parser.Configure(opt => opt.Option3)
                .Name("3")
                .Default(defaultValue)
                .Required();

            var parsed = parser.Parse(args);

            parsed.AssertNoErrors();

            Assert.Equal(result1, parsed.Result.Option1);
            Assert.Equal(result2, parsed.Result.Option2);
            Assert.Equal(result3, parsed.Result.Option3);
        }

        [Fact]
        public async Task ParseWithCommandTestsAsync()
        {
            var wait = new ManualResetEvent(false);

            var parser = new CommandLineParser<Options>(Services);

            parser.Configure(opt => opt.Option1)
                .Name("o")
                .Default("Default message")
                .Required();

            var addCmd = parser.AddCommand<AddOption>()
                .Name("add")
                .OnExecutingAsync(async (opt, cmdOpt, ctx) =>
                {
                    await Task.Delay(100);

                    Assert.Equal("test", opt.Option1);
                    Assert.Equal("my message", cmdOpt.Message);

                    await Task.Delay(100);

                    wait.Set();

                    await Task.Delay(100);
                });

            addCmd.Configure(opt => opt.Message)
                .Name("m", "message")
                .Required();

            var parsed = await parser.ParseAsync(new string[] { "app.exe", "-o", "test", "add", "-m=my message" });

            parsed.AssertNoErrors();

            Assert.Equal("test", parsed.Result.Option1);

            parsed.ExecuteCommandsAsync(default).GetAwaiter().GetResult();

            Assert.True(wait.WaitOne(2000));
        }

        [Theory]
        [InlineData(new[] { "app.exe", "add", "-m", "message2", "-m", "message1" }, "message1", "message2")]
        [InlineData(new[] { "app.exe", "-m", "message1", "add", "-m", "message2" }, "message1", "message2")]
        [InlineData(new[] { "add", "-m", "message2", "-m", "message1" }, "message1", "message2")]
        [InlineData(new[] { "-m", "message1", "add", "-m", "message2" }, "message1", "message2")]
        public void ParseCommandTests(string[] args, string result1, string result2)
        {
            var parser = new CommandLineParser<AddOption>(Services);
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
        [InlineData(new[] { "app.exe", "add", "-m", "message2", "-m", "message1" }, "message1", "message2")]
        [InlineData(new[] { "app.exe", "-m", "message1", "add", "-m", "message2" }, "message1", "message2")]
        [InlineData(new[] { "add", "-m", "message2", "-m", "message1" }, "message1", "message2")]
        [InlineData(new[] { "-m", "message1", "add", "-m", "message2" }, "message1", "message2")]
        public async Task ParseCommandTestsAsync(string[] args, string result1, string result2)
        {
            var parser = new CommandLineParser<AddOption>(Services);
            var wait = new ManualResetEvent(false);

            parser.AddCommand<AddOption>()
                .Name("add")
                .Required()
                .OnExecutingAsync(async (opt1, opt2, ctx) =>
                {
                    await Task.Delay(100);

                    wait.Set();

                    Assert.Equal(result2, opt2.Message);
                })
                .Configure(c => c.Message)
                    .Name("m", "message")
                    .Required();

            parser.Configure(opt => opt.Message)
                .Name("m", "message")
                .Required();

            var result = await parser.ParseAsync(args);

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
            var parser = new CommandLineParser<Options>(Services);

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
            var parser = new CommandLineParser<Model_Issue_35>(Services);

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
            var parser = new CommandLineParser<Options>(Services);

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

        [Theory]
        [InlineData(new string[] { "" }, "defaulttransformed", false)]
        [InlineData(new string[] { "-m", "test" }, "testtransformed", false)]
        [InlineData(new string[] { "--message", "test" }, "testtransformed", false)]
        public void TransformationWorksAsExpected(string[] args, string expected, bool errors)
        {
            var parser = new CommandLineParser<AddOption>(Services);

            parser.Configure(a => a.Message)
                .Name("m", "message")
                .Required()
                .Transform(msg => $"{msg}transformed")
                .Default("default");

            var result = parser.Parse(args);

            Assert.Equal(errors, result.AssertNoErrors(false));

            Assert.Equal(expected, result.Result.Message);
        }

        [Theory]
        [InlineData(new string[] { "" }, 11, false)]
        [InlineData(new string[] { "-i", "10" }, 20, false)]
        [InlineData(new string[] { "--int", "10" }, 20, false)]
        public void TransformationWorksAsExpectedForInts(string[] args, int expected, bool errors)
        {
            var parser = new CommandLineParser<IntOptions>(Services);

            parser.Configure(a => a.SomeInt)
                .Name("i", "int")
                .Required()
                .Transform(value => value + 10)
                .Default(1);

            var result = parser.Parse(args);

            Assert.Equal(errors, result.AssertNoErrors(false));

            Assert.Equal(expected, result.Result.SomeInt);
        }

        [Theory]
        [InlineData(new string[] { "cmd" }, 11, false)]
        [InlineData(new string[] { "cmd", "-i", "10" }, 20, false)]
        [InlineData(new string[] { "cmd", "--int", "10" }, 20, false)]
        public void TransformationWorksAsExpectedForCommandOptions(string[] args, int expected, bool errors)
        {
            int outcome = -1;

            var parser = new CommandLineParser(Services);

            var cmd = parser.AddCommand<IntOptions>()
                .Name("cmd")
                .Required()
                .OnExecuting((_, i) => outcome = i.SomeInt);

            cmd.Configure(a => a.SomeInt)
                .Name("i", "int")
                .Required()
                .Transform(value => value + 10)
                .Default(1);

            var result = parser.Parse(args);

            Assert.Equal(errors, result.AssertNoErrors(false));

            Assert.Equal(expected, outcome);
        }

        [Theory]
        [InlineData(new string[] { "cmd" }, "", true)]
        [InlineData(new string[] { "cmd", "-s", "-s2" }, "", true)]
        [InlineData(new string[] { "cmd", "-s", "test", "-s2", "test" }, "test", false)]
        [InlineData(new string[] { "cmd", "--string", "test", "-s2", "test" }, "test", false)]
        public void CustomTypeWithStringTryParseGetsParsedCorrectly(string[] args, string expected, bool errors)
        {
            var parser = new CommandLineParser<StringTryParseTypeOptions>(Services);

            var result = parser.Parse(args);

            Assert.Equal(errors, result.AssertNoErrors(!errors));

            if (!result.HasErrors)
            {
                Assert.Equal(expected, result.Result.String.Result);
                Assert.Equal(expected, result.Result.String2.Result);
            }
        }

        [Theory]
        [InlineData(new string[] { "cmd" }, "", true)]
        [InlineData(new string[] { "cmd", "-s", "-s2", "-s3" }, "", true)]
        [InlineData(new string[] { "cmd", "-s", "test", "-s2", "test", "-s3", "test" }, "test", false)]
        [InlineData(new string[] { "cmd", "--string", "test", "-s2", "test", "-s3", "test" }, "test", false)]
        public void CustomTypeWithStringConstructorGetsParsedCorrectly(string[] args, string expected, bool errors)
        {
            var parser = new CommandLineParser<StringTypeOptions>(Services);

            var result = parser.Parse(args);

            Assert.Equal(errors, result.AssertNoErrors(!errors));

            if (!result.HasErrors)
            {
                Assert.Equal(expected, result.Result.String.Result);
                Assert.Equal(expected, result.Result.String2.Result);
                Assert.Equal(expected, result.Result.String3.Result);
            }
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

        private class OptionsWithThreeParams<T>
        {
            public T Option1 { get; set; }
            public T Option2 { get; set; }
            public T Option3 { get; set; }
        }

        private class IntOptions
        {
            public int SomeInt { get; set; }
        }

        private class StringTypeOptions
        {
            [Name("s", "string"), Required]
            public StringType String { get; set; }

            [Name("s2"), Required]
            public StringType4 String2 { get; set; }

            [Name("s3"), Required]
            public StringType5 String3 { get; set; }
        }

        private class StringTryParseTypeOptions
        {
            [Name("s", "string"), Required]
            public StringType2 String { get; set; }

            [Name("s2"), Required]
            public StringType3 String2 { get; set; }
        }

        private class StringType
        {
            public StringType(string input)
            {
                Result = input;
            }

            public StringType(string input, string input2)
            {
                Result = input;
            }

            public string Result { get; }
        }

        private class StringType2
        {
            private StringType2(string input)
            {
                Result = input;
            }

            public string Result { get; }

#pragma warning disable IDE0060 // Remove unused parameter
            public static bool TryParse(string input, IFormatProvider format, out StringType2 result)
            {
                result = new StringType2(input);
                return true;
            }

            public static bool TryParse() => false;

            public static void Tryparse(string input, IFormatProvider format, out StringType2 result)
            {
                result = default;
            }

            public static bool TryParse(string input, StringType2 xd, out StringType2 stringType2)
            {
                stringType2 = default;
                return false;
            }
        }

        private class StringType3
        {
            private StringType3(string input)
            {
                Result = input;
            }

            public string Result { get; }

            public static bool TryParse(string input, out StringType3 result)
            {
                result = new StringType3(input);
                return true;
            }
        }

        private class StringType4
        {
            private StringType4(string input)
            {
                Result = input;
            }

            public string Result { get; }

            public static StringType4 Parse(string input)
            {
                return new StringType4(input);
            }
        }

        private class StringType5
        {
            private StringType5(string input)
            {
                Result = input;
            }

            public string Result { get; }

            public static StringType5 Parse(string input, IFormatProvider provider)
            {
                return new StringType5(input);
            }

            public static StringType4 Parse(string input)
            {
                return null;
            }

            public static StringType5 Parse(string input, IFormatProvider provider, IFormatProvider xd)
            {
                return null;
            }
#pragma warning restore IDE0060 // Remove unused parameter
        }

        private class OrderModel
        {
            [OptionOrder(1), Required]
            public string From { get; set; }

            [OptionOrder(2), Required]
            public string To { get; set; }

            [Name("r"), DefaultValue(5)]
            public int Random { get; set; }
        }

        private class OrderModelInt
        {
            [OptionOrder(1), Required]
            public int From { get; set; }

            [OptionOrder(2), Required]
            public int To { get; set; }

            [Name("r"), DefaultValue(5)]
            public int Random { get; set; }
        }
    }
}
