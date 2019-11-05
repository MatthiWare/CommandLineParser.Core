using System;

namespace MatthiWare.CommandLine.Abstractions.Validations
{
    public interface IValidationResult
    {
        bool IsValid { get; }
        Exception Error { get; }
    }
}
