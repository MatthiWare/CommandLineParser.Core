namespace MatthiWare.CommandLine.Abstractions.Validations
{
    public interface IValidator<T> : IValidator
    {
        IValidationResult Validate(T @object);
    }
}
