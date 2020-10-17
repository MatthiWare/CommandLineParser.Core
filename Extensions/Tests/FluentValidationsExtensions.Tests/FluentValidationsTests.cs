using FluentValidationsExtensions.Tests.Commands;
using FluentValidationsExtensions.Tests.Models;
using FluentValidationsExtensions.Tests.Validators;
using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Extensions.FluentValidations;
using MatthiWare.CommandLine.Extensions.FluentValidations.Core;
using MatthiWare.CommandLine.Tests;
using Xunit;
using Xunit.Abstractions;

namespace FluentValidationsExtensions.Tests
{
    public class FluentValidationsTests : TestBase
    {
        private static readonly EmailModelValidator emailModelValidator = new EmailModelValidator();
        private static readonly FirstModelValidator firstModelValidator = new FirstModelValidator();

        public FluentValidationsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void FluentValidationsShouldWork(bool useGeneric, bool useInstance)
        {
            var parser = new CommandLineParser<FirstModel>(Services);

            parser.UseFluentValidations(config =>
            {
                OnConfigureFluentValidations(config, useGeneric, useInstance);
            });

            parser.RegisterCommand<CommandWithModel<FirstModel, EmailModel>, EmailModel>();

            var result = parser.Parse(new string[] { "-f", "JamesJames1", "-l", "Bond123456789", "cmd", "-e", "jane.doe@provider.com", "-i", "50" });

            result.AssertNoErrors();
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void WrongArgumentsShouldThrowValidationError(bool useGeneric, bool useInstance)
        {
            var parser = new CommandLineParser<FirstModel>(Services);

            parser.UseFluentValidations(config =>
            {
                OnConfigureFluentValidations(config, useGeneric, useInstance);
            });

            parser.RegisterCommand<CommandWithModel<FirstModel, EmailModel>, EmailModel>();

            var result = parser.Parse(new string[] { "-f", "James", "-l", "Bond", "cmd", "-e", "jane.doe@provider.com", "-i", "50" });

            Assert.True(result.AssertNoErrors(false));
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void SubCommandShouldFailIfValidationFailsForModel(bool useGeneric, bool useInstance)
        {
            var parser = new CommandLineParser<FirstModel>(Services);

            parser.UseFluentValidations(config =>
            {
                OnConfigureFluentValidations(config, useGeneric, useInstance);
            });

            parser.RegisterCommand<CommandWithModel<FirstModel, EmailModel>, EmailModel>();

            var result = parser.Parse(new string[] { "-f", "JamesJames1", "-l", "BondBond1231456", "cmd", "-e", "jane.doe@provider.com", "-i", "0" });

            Assert.True(result.AssertNoErrors(false));
        }

        private void OnConfigureFluentValidations(FluentValidationConfiguration config, bool useGeneric, bool useInstantiated)
        {
            if (useGeneric)
            {
                if (useInstantiated)
                {
                    config
                        .AddValidatorInstance<EmailModel, EmailModelValidator>(emailModelValidator)
                        .AddValidatorInstance<FirstModel, FirstModelValidator>(firstModelValidator);
                }
                else
                {
                    config
                        .AddValidator<EmailModel, EmailModelValidator>()
                        .AddValidator<FirstModel, FirstModelValidator>();
                }
            }
            else
            {
                if (useInstantiated)
                {
                    config
                        .AddValidatorInstance(typeof(EmailModel), emailModelValidator)
                        .AddValidatorInstance(typeof(FirstModel), firstModelValidator);
                }
                else
                {
                    config
                        .AddValidator(typeof(EmailModel), typeof(EmailModelValidator))
                        .AddValidator(typeof(FirstModel), typeof(FirstModelValidator));
                }
            }
        }
    }
}
