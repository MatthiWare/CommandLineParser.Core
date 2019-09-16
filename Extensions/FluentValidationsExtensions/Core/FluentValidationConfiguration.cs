using FluentValidation;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.CommandLine.Extensions.FluentValidations.Core
{
    public sealed class FluentValidationConfiguration : ValidationConfigurationBase
    {
        private readonly IContainerResolver resolver;

        public FluentValidationConfiguration(IValidatorsContainer container, IContainerResolver resolver)
            : base(container)
        {
            this.resolver = resolver;
        }

        public FluentValidationConfiguration AddValidator<K, V>()
            where V : AbstractValidator<K>, new()
        {
            V instance = resolver.Resolve<V>();

            var validator = new FluentTypeValidatorCollection<K>(resolver);

            Validators.AddValidator(validator);

            return this;
        }

        public FluentValidationConfiguration AddValidatorInstance<K, V>(V instance)
            where V : AbstractValidator<K>, new()
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var validator = new FluentTypeValidatorCollection<K>(resolver);

            Validators.AddValidator(validator);

            return this;
        }

        public override IValidationResult Validate(object @object)
        {
            return null;
        }
    }
}
