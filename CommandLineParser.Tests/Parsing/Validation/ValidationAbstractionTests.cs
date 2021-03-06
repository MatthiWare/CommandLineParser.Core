﻿using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Validations;
using MatthiWare.CommandLine.Core.Attributes;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Parsing.Validation
{
    public class ValidationAbstractionTests : TestBase
    {
        public ValidationAbstractionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void ParsingCallsValidation()
        {
            Services.AddCommandLineParser<OptionWithCommand>();
            var parser = ResolveParser<OptionWithCommand>();

            var validValidationResultMock = new Mock<IValidationResult>();
            validValidationResultMock.SetupGet(v => v.IsValid).Returns(true);

            var optionWithCommandMockValidator = new Mock<IValidator<OptionWithCommand>>();
            optionWithCommandMockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validValidationResultMock.Object)
                .Verifiable();

            var optionMockValidator = new Mock<IValidator<Option>>();
            optionMockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validValidationResultMock.Object)
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
            Services.AddCommandLineParser<OptionWithCommand>();
            var parser = ResolveParser<OptionWithCommand>();

            var validValidationResultMock = new Mock<IValidationResult>();
            validValidationResultMock.SetupGet(v => v.IsValid).Returns(true);

            var optionWithCommandMockValidator = new Mock<IValidator<OptionWithCommand>>();
            optionWithCommandMockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<OptionWithCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validValidationResultMock.Object)
                .Verifiable();

            var optionMockValidator = new Mock<IValidator<Option>>();
            optionMockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Option>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validValidationResultMock.Object)
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
