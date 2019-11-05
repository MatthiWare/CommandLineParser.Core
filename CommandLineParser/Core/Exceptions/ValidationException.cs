using System;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(Exception innerException) 
            : base(innerException.Message, innerException)
        {
        }
    }
}
