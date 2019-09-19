using FluentValidationsExtensions.Tests.Models;
using MatthiWare.CommandLine;
using Xunit;
using MatthiWare.CommandLine.Extensions.FluentValidations;
using FluentValidationsExtensions.Tests.Validators;
using FluentValidationsExtensions.Tests.Commands;
using MatthiWare.CommandLine.Tests;

namespace FluentValidationsExtensions.Tests
{
    public class FluentValidationsTests
    {
        [Fact]
        public void FluentValidationsShouldWork()
        {
            var parser = new CommandLineParser<FirstModel>();

            parser.UseFluentValidations(config =>
            {
                config
                .AddValidator<EmailModel, EmailModelValidator>()
                .AddValidator<FirstModel, FirstModelValidator>();
            });

            parser.RegisterCommand<CommandWithModel<FirstModel, EmailModel>, EmailModel>();

            var result = parser.Parse(new string[] { "app.exe", "-f", "JamesJames1", "-l", "Bond123456789", "cmd", "-e", "jane.doe@provider.com", "-i", "50" });

            result.AssertNoErrors();
        }

        [Fact]
        public void WrongArgumentsShouldThrowValidationError()
        {
            var parser = new CommandLineParser<FirstModel>();

            parser.UseFluentValidations(config =>
            {
                config
                .AddValidator<EmailModel, EmailModelValidator>()
                .AddValidator<FirstModel, FirstModelValidator>();
            });

            parser.RegisterCommand<CommandWithModel<FirstModel, EmailModel>, EmailModel>();

            var result = parser.Parse(new string[] { "app.exe", "-f", "James", "-l", "Bond", "cmd", "-e", "jane.doe@provider.com", "-i", "50" });

            Assert.True(result.AssertNoErrors(false));
        }

        [Fact]
        public void SubCommandShouldFailIfValidationFailsForModel()
        {
            var parser = new CommandLineParser<FirstModel>();

            parser.UseFluentValidations(config =>
            {
                config
                .AddValidator<EmailModel, EmailModelValidator>()
                .AddValidator<FirstModel, FirstModelValidator>();
            });

            parser.RegisterCommand<CommandWithModel<FirstModel, EmailModel>, EmailModel>();

            var result = parser.Parse(new string[] { "app.exe", "-f", "JamesJames1", "-l", "BondBond1231456", "cmd", "-e", "jane.doe@provider.com", "-i", "0" });

            Assert.True(result.AssertNoErrors(false));
        }
    }
}
