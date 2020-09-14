using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Command
{
    public class CommandInModelTests : TestBase
    {
        public CommandInModelTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }


        [Fact]
        public async Task FindCommandsInModels()
        {
            var parser = new CommandLineParser<ModelWithCommands>(Services);

            Assert.Equal(3, parser.Commands.Count);
        }

        public class ModelWithCommands
        {
            public NonGenericCommand NonGenericCommand { get; set; }
            public GenericCommandWithParentOptions GenericCommandWithParentOptions { get; set; }
            public GenericCommandWithOwnOptions GenericCommandWithOwnOptions { get; set; }
        }

        public class ModelWithOptions
        {
            [Name("i", "input")]
            public string MyOption { get; set; }
        }

        public class NonGenericCommand : Abstractions.Command.Command
        {
            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                builder.Name(nameof(NonGenericCommand));
            }
        }

        public class GenericCommandWithParentOptions : Command<ModelWithCommands>
        {
            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                builder.Name(nameof(GenericCommandWithParentOptions));
            }
        }

        public class GenericCommandWithOwnOptions : Command<ModelWithCommands, ModelWithOptions>
        {
            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                builder.Name(nameof(GenericCommandWithOwnOptions));
            }
        }
    }
}
