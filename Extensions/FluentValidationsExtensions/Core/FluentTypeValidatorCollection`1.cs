using FluentValidation;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Validations;
using MatthiWare.CommandLine.Core;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MatthiWare.CommandLine.Extensions.FluentValidations.Core
{
    internal class FluentTypeValidatorCollection : Abstractions.Validations.IValidator
    {
        private readonly TypedInstanceCache<FluentValidation.IValidator> validators;

        public FluentTypeValidatorCollection(IServiceProvider serviceProvider)
        {
            validators = new TypedInstanceCache<FluentValidation.IValidator>(serviceProvider);
        }

        public void AddValidator(FluentValidation.IValidator validator)
        {
            validators.Add(validator);
        }

        public void AddValidator(Type t) => validators.Add(t);

        public void AddValidator<K>() where K : FluentValidation.IValidator
            => AddValidator(typeof(K));

        public IValidationResult Validate(object @object)
        {
            var errors = validators.Get()
                .Select(v => v.Validate(new ValidationContext<object>(@object)))
                .SelectMany(r => r.Errors)
                .ToList();

            if (errors.Any())
            {
                return FluentValidationsResult.Failure(errors);
            }
            else
            {
                return FluentValidationsResult.Succes();
            }
        }

        public async Task<IValidationResult> ValidateAsync(object @object, CancellationToken cancellationToken = default)
        {
            var errors = (await Task.WhenAll(validators.Get()
                .Select(async v => await v.ValidateAsync(new ValidationContext<object>(@object), cancellationToken))))
                .SelectMany(r => r.Errors)
                .ToList();

            if (errors.Any())
            {
                return FluentValidationsResult.Failure(errors);
            }
            else
            {
                return FluentValidationsResult.Succes();
            }
        }
    }
}
