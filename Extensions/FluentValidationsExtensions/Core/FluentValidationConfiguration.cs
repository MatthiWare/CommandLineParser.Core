using FluentValidation;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Extensions.FluentValidations.Core
{
    public sealed class FluentValidationConfiguration
    {
        private readonly IValidatorsContainer container;

        public FluentValidationConfiguration(IValidatorsContainer container)
        {
            this.container = container;
        }

        public FluentValidationConfiguration AddValidator<K, V>()
            where V : AbstractValidator<K>, new()
        {
            var validator = new Validator<K>(typeof(V));

            container.AddValidator(validator);

            return this;
        }

        public FluentValidationConfiguration AddValidatorInstance<K, V>(V instance)
            where V : AbstractValidator<K>, new()
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var validator = new Validator<K>(instance);

            container.AddValidator(validator);

            return this;
        }
    }
}
