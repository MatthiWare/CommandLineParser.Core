using FluentValidationsExtensions.Tests.Models;
using MatthiWare.CommandLine;
using Xunit;
using MatthiWare.CommandLine.Extensions.FluentValidations;
using FluentValidationsExtensions.Tests.Validators;
using FluentValidationsExtensions.Tests.Commands;
using MatthiWare.CommandLine.Tests;

namespace FluentValidationsExtensions.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var parser = new CommandLineParser<FirstModel>();

            parser.UseFluentValidations(config =>
            {
                config
                .AddValidator<EmailModel, EmailModelValidator>()
                .AddValidator<FirstModel, FirstModelValidator>();
            });

            parser.AddCommand<CommandWithModel<FirstModel, EmailModel>>();

            var result = parser.Parse(new string[] { "app.exe", "-f", "James", "-l", "Bond", "cmd", "-e", "jane.doe@provider.com", "-i", "50" });

            result.AssertNoErrors();
        }
    }
}
