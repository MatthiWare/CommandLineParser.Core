using FluentValidation;
using FluentValidationsExtensions.Tests.Models;

namespace FluentValidationsExtensions.Tests.Validators
{
    public class ValidatorWithDependency : AbstractValidator<EmailModel>
    {
        public ValidatorWithDependency(IValidationDependency dependency)
        {
            RuleFor(_ => _.Email).Must(dependency.IsValid);
        }
    }

    public interface IValidationDependency
    {
        bool IsValid(string input);
    }
}
