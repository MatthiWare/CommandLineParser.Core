﻿using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Core.Attributes;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MatthiWare.CommandLine.Tests.Command
{
    public class CommandInModelTests : TestBase
    {
        public CommandInModelTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        #region FindCommandsInModel

        [Fact(Timeout = 1000)]
        public void FindCommandsInModel()
        {
            Services.AddLogging();
            Services.AddCommandLineParser<ModelWithCommands>();

            var parser = ResolveParser<ModelWithCommands>();

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

        #endregion

        #region SubCommandFindCommandsInModel

        [Fact]
        public void FindCommandsInCommandModel()
        {
            Services.AddCommandLineParser();

            var parser = ResolveParser();

            parser.RegisterCommand<GenericSubCommandWithOwnOptions, SubCommandModelWithCommands>();

            Assert.Equal(3, ((ICommandLineCommandContainer)parser.Commands[0]).Commands.Count);
        }

        public class SubCommandModelWithCommands
        {
            public NonGenericCommand NonGenericCommand { get; set; }
            public SubCommandWithModelOptions SubCommandWithModelOptions { get; set; }
            public SimpleGenericCommand SimpleGenericCommand { get; set; }
        }

        public class GenericSubCommandWithOwnOptions : Command<object, SubCommandModelWithCommands>
        {
            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                builder.Name(nameof(GenericSubCommandWithOwnOptions));
            }
        }

        public class SubCommandWithModelOptions : Command<object, ModelWithOptions>
        {
            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                builder.Name(nameof(SubCommandWithModelOptions));
            }
        }

        public class SimpleGenericCommand : Command<object>
        {
            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                builder.Name(nameof(SimpleGenericCommand));
            }
        }

        #endregion
    }
}
