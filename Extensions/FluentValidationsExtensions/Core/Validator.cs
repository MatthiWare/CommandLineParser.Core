using FluentValidation;
using MatthiWare.CommandLine.Abstractions.Validations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.CommandLine.Extensions.FluentValidations.Core
{
    internal class Validator<T> : Abstractions.Validations.IValidator<T>
    {
        private List<FluentValidation.IValidator> validators;
        private IDictionary<Type, Type> m_types = new Dictionary<Type, Type>();
        private IDictionary<Type, FluentValidation.IValidator> m_cache = new Dictionary<Type, FluentValidation.IValidator>();

        public Validator(FluentValidation.IValidator<T> validator)
        {
            m_cache.Add(typeof(T), validator);
        }

        public Validator(Type validatorType)
        {
            m_types.Add(typeof(T), validatorType);
        }

        public IValidationResult Validate(T @object)
        {
            var errors = validators
                .Select(v => v.Validate(@object))
                .SelectMany(r => r.Errors)
                .Where(e => e != null)
                .ToList();

            if (errors.Any())
            {
                var exception = new ValidationException(errors);

                return new FluentValidationsResult(new CommandLine.Core.Exceptions.ValidationException(exception));
            }
            else
            {
                return new FluentValidationsResult();
            }
        }

        public IValidationResult Validate(object @object) => Validate((T)@object);
    }
}
