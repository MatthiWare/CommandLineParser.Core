using FluentValidation;
using FluentValidationsExtensions.Tests.Models;

namespace FluentValidationsExtensions.Tests.Validators
{
    public class FirstModelValidator : AbstractValidator<FirstModel>
    {
        public FirstModelValidator()
        {
            RuleFor(model => model.FirstName).MaximumLength(30).MinimumLength(10).NotEmpty();
            RuleFor(model => model.LastName).MaximumLength(30).MinimumLength(10).NotEmpty();
        }
    }
}
