using FluentValidation.Results;
using MatthiWare.CommandLine.Abstractions.Validations;
using MatthiWare.CommandLine.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Extensions.FluentValidations.Core
{
    internal sealed class FluentValidationsResult : IValidationResult
    {
        private FluentValidationsResult(IEnumerable<ValidationFailure> errors = null)
        {
            if (errors != null)
            {
                Error = new FluentValidation.ValidationException(errors);
            }

            IsValid = Error == null;
        }

        public bool IsValid { get; }

        public Exception Error { get; }

        public static FluentValidationsResult Succes() => new FluentValidationsResult();

        public static FluentValidationsResult Failure(IEnumerable<ValidationFailure> errors) => new FluentValidationsResult(errors);
    }
}
