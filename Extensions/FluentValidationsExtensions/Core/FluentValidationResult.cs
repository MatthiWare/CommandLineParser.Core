using MatthiWare.CommandLine.Abstractions.Validations;
using MatthiWare.CommandLine.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Extensions.FluentValidations.Core
{
    internal sealed class FluentValidationsResult : IValidationResult
    {
        public FluentValidationsResult(ValidationException validationException = null)
        {
            Error = validationException;
            IsValid = validationException == null;
        }

        public bool IsValid { get; }

        public Exception Error { get; }
    }
}
