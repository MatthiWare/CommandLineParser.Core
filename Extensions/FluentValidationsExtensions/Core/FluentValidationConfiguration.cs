using FluentValidation;
using MatthiWare.CommandLine.Abstractions.Validations;
using MatthiWare.CommandLine.Core.Utils;
using System;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Extensions.FluentValidations.Core
{
    /// <summary>
    /// Configuration for fluent validations
    /// </summary>
    public sealed class FluentValidationConfiguration : ValidationConfigurationBase
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Dictionary<Type, FluentTypeValidatorCollection> validators = new Dictionary<Type, FluentTypeValidatorCollection>();

        /// <summary>
        /// Creates a new fluent validation configuration
        /// </summary>
        /// <param name="container"></param>
        /// <param name="serviceProvider"></param>
        public FluentValidationConfiguration(IValidatorsContainer container, IServiceProvider serviceProvider)
            : base(container)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Adds a validator 
        /// </summary>
        /// <param name="key">type to validate</param>
        /// <param name="validator">Validator type</param>
        /// <returns>Self</returns>
        public FluentValidationConfiguration AddValidator(Type key, Type validator)
        {
            if (!validator.IsAssignableToGenericType(typeof(FluentValidation.IValidator<>)))
            {
                throw new InvalidCastException($"{validator} is not assignable to {typeof(FluentValidation.IValidator<>)}");
            }

            GetValidatorCollection(key).AddValidator(validator);

            return this;
        }

        /// <summary>
        /// Adds a validator 
        /// </summary>
        /// <typeparam name="K">Type to validate</typeparam>
        /// <typeparam name="V">Validator type</typeparam>
        /// <returns>Self</returns>
        public FluentValidationConfiguration AddValidator<K, V>()
            where V : AbstractValidator<K>, new()
        {
            GetValidatorCollection(typeof(K)).AddValidator<V>();

            return this;
        }

        /// <summary>
        /// Adds an instantiated validator
        /// </summary>
        /// <typeparam name="K">Type to validate</typeparam>
        /// <typeparam name="V">Validator type</typeparam>
        /// <param name="instance">Instance</param>
        /// <returns>Self</returns>
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

        /// <summary>
        /// Add an instantiated validator
        /// </summary>
        /// <param name="key">Type to validate</param>
        /// <param name="instance">Validator instance</param>
        /// <returns>Self</returns>
        public FluentValidationConfiguration AddValidatorInstance(Type key, FluentValidation.IValidator instance)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            GetValidatorCollection(key).AddValidator(instance);

            return this;
        }

        private FluentTypeValidatorCollection GetValidatorCollection(Type key)
        {
            if (!validators.TryGetValue(key, out FluentTypeValidatorCollection validator))
            {
                validator = new FluentTypeValidatorCollection(serviceProvider);

                validators.Add(key, validator);
                Validators.AddValidator(key, validator);
            }

            return validator;
        }
    }
}
