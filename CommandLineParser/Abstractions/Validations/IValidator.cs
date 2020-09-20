using System.Threading;
using System.Threading.Tasks;

namespace MatthiWare.CommandLine.Abstractions.Validations
{
    /// <summary>
    /// Validator
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validates an object
        /// </summary>
        /// <param name="object">Item to validate</param>
        /// <returns><see cref="IValidationResult"/></returns>
        IValidationResult Validate(object @object);

        /// <summary>
        /// Validates an object async
        /// </summary>
        /// <param name="object">Item to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="IValidationResult"/></returns>
        Task<IValidationResult> ValidateAsync(object @object, CancellationToken cancellationToken);
    }
}
