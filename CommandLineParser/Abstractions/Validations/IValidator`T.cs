namespace MatthiWare.CommandLine.Abstractions.Validations
{
    /// <inheritdoc/>
    public interface IValidator<T> : IValidator
    {
        /// <summary>
        /// Validates an object
        /// </summary>
        /// <param name="object">Item to validate</param>
        /// <returns><see cref="IValidationResult"/></returns>
        IValidationResult Validate(T @object);
    }
}
