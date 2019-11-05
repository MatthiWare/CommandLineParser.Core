using FluentValidation;
using FluentValidationsExtensions.Tests.Models;

namespace FluentValidationsExtensions.Tests.Validators
{
    public class EmailModelValidator : AbstractValidator<EmailModel>
    {
        public EmailModelValidator()
        {
            RuleFor(model => model.Email).EmailAddress().NotEmpty();
            RuleFor(model => model.Id).NotEmpty().InclusiveBetween(5, 100);
        }
    }
}
