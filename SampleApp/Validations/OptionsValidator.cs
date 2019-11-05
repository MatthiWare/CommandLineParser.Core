using FluentValidation;

namespace SampleApp.Validations
{
    public class OptionsValidator : AbstractValidator<Program.Options>
    {
        public OptionsValidator()
        {
            RuleFor(o => o.MyInt).InclusiveBetween(5, 10);
            RuleFor(o => o.MyString).MinimumLength(10).MaximumLength(20);
        }
    }
}
