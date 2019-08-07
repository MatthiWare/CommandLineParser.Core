using FluentValidation;

namespace SampleApp.Validations
{
    public class OptionsValidator : AbstractValidator<Program.Options>
    {
        public OptionsValidator()
        {
            RuleFor(o => o.MyInt).InclusiveBetween(1, 10);
            RuleFor(o => o.MyString).MinimumLength(5).MaximumLength(20);
        }
    }
}
