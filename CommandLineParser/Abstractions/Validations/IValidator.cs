using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions.Validations
{
    public interface IValidator
    {
        IValidationResult Validate(object @object);
    }
}
