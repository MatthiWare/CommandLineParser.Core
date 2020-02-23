using System.Threading;
using System.Threading.Tasks;

namespace MatthiWare.CommandLine.Abstractions.Validations
{
    public interface IValidator
    {
        IValidationResult Validate(object @object);

        Task<IValidationResult> ValidateAsync(object @object, CancellationToken cancellationToken);
    }
}
