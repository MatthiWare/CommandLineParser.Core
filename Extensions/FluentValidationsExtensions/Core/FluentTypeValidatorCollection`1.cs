using FluentValidation;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Validations;
using MatthiWare.CommandLine.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.CommandLine.Extensions.FluentValidations.Core
{
    internal class FluentTypeValidatorCollection<T> : Abstractions.Validations.IValidator<T>
    {
        private readonly TypedInstanceCache<FluentValidation.IValidator<T>> validators;

        public FluentTypeValidatorCollection(IContainerResolver resolver)
        {
            validators = new TypedInstanceCache<FluentValidation.IValidator<T>>(resolver);
        }

        public void AddValidator(FluentValidation.IValidator<T> validator)
        {
            validators.Add(validator);
        }

        public void AddValidator(Type t) => validators.Add(t);

        public void AddValidator<K>() where K : FluentValidation.IValidator<T>
            => AddValidator(typeof(K));

        public IValidationResult Validate(T @object)
        {
            var errors = validators.Get()
                .Select(v => v.Validate(@object))
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

        public IValidationResult Validate(object @object) => Validate((T)@object);
    }
}
