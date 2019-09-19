using FluentValidation;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Validations;
using MatthiWare.CommandLine.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.CommandLine.Extensions.FluentValidations.Core
{
    public sealed class FluentValidationConfiguration : ValidationConfigurationBase
    {
        private readonly IContainerResolver resolver;
        private readonly Dictionary<Type, FluentTypeValidatorCollection> validators = new Dictionary<Type, FluentTypeValidatorCollection>();

        public FluentValidationConfiguration(IValidatorsContainer container, IContainerResolver resolver)
            : base(container)
        {
            this.resolver = resolver;
        }

        public FluentValidationConfiguration AddValidator(Type key, Type validator)
        {
            if (!validator.IsAssignableToGenericType(typeof(FluentValidation.IValidator<>)))
            {
                throw new InvalidCastException($"{validator} is not assignable to {typeof(FluentValidation.IValidator<>)}");
            }

            GetValidatorCollection(key).AddValidator(validator);

            return this;
        }

        public FluentValidationConfiguration AddValidator<K, V>()
            where V : AbstractValidator<K>, new()
        {
            GetValidatorCollection(typeof(K)).AddValidator<V>();

            return this;
        }

        public FluentValidationConfiguration AddValidatorInstance<K, V>(V instance)
            where V : AbstractValidator<K>, new()
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            GetValidatorCollection(typeof(K)).AddValidator(instance);

            return this;
        }

        private FluentTypeValidatorCollection GetValidatorCollection(Type key)
        {
            if (!validators.TryGetValue(key, out FluentTypeValidatorCollection validator))
            {
                validator = new FluentTypeValidatorCollection(resolver);

                validators.Add(key, validator);
                Validators.AddValidator(key, validator);
            }

            return validator;
        }
    }
}
