using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Validations;
using MatthiWare.CommandLine.Core.Attributes;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Parsing.Validation
{
    public class ValidationAbstractionTests
    {
        [Fact]
        public void ParsingCallsValidation()
        {
            var parser = new CommandLineParser<OptionWithCommand>();

            var validValidationResultMock = new Mock<IValidationResult>();
            validValidationResultMock.SetupGet(v => v.IsValid).Returns(true);

            var optionWithCommandMockValidator = new Mock<IValidator<OptionWithCommand>>();
            optionWithCommandMockValidator
                .Setup(v => v.Validate(It.IsAny<object>()))
                .Returns(validValidationResultMock.Object)
                .Verifiable();

            var optionMockValidator = new Mock<IValidator<Option>>();
            optionMockValidator
                .Setup(v => v.Validate(It.IsAny<object>()))
                .Returns(validValidationResultMock.Object)
                .Verifiable();

            parser.Validators.AddValidator(optionWithCommandMockValidator.Object);
            parser.Validators.AddValidator(optionMockValidator.Object);

            var result = parser.Parse(new[] { "-x", "true", "cmd", "-y", "true" });

            result.AssertNoErrors();

            optionMockValidator.Verify();
            optionWithCommandMockValidator.Verify();
        }

        [Fact]
        public async Task ParsingCallsValidationAsync()
        {
            var parser = new CommandLineParser<OptionWithCommand>();

            var validValidationResultMock = new Mock<IValidationResult>();
            validValidationResultMock.SetupGet(v => v.IsValid).Returns(true);

            var optionWithCommandMockValidator = new Mock<IValidator<OptionWithCommand>>();
            optionWithCommandMockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<OptionWithCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(validValidationResultMock.Object))
                .Verifiable();

            var optionMockValidator = new Mock<IValidator<Option>>();
            optionMockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Option>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(validValidationResultMock.Object))
                .Verifiable();

            parser.Validators.AddValidator(optionWithCommandMockValidator.Object);
            parser.Validators.AddValidator(optionMockValidator.Object);

            var result = await parser.ParseAsync(new[] { "-x", "true", "cmd", "-y", "true" });

            result.AssertNoErrors();

            optionMockValidator.Verify();
            optionWithCommandMockValidator.Verify();
        }

        public class OptionWithCommand
        {
            [Name("x")]
            public bool Prop { get; set; }

            public Cmd Cmd { get; set; }

        }

        public class Option
        {
            [Name("y")]
            public bool Prop { get; set; }
        }

        public class Cmd : Command<OptionWithCommand, Option>
        {
            public override void OnConfigure(ICommandConfigurationBuilder<Option> builder)
            {
                base.OnConfigure(builder);

                builder.Name("cmd");
            }

            public override void OnConfigure(ICommandConfigurationBuilder builder)
            {
                base.OnConfigure(builder);

                builder.Name("cmd");
            }
        }
    }
}
