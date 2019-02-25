using System;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Base exception class that exposes the <see cref="Argument"/> this exception is about. 
    /// </summary>
    public abstract class BaseParserException : Exception
    {
        /// <summary>
        /// The argument this exception is for. 
        /// <see cref="ICommandLineOption"/> and <see cref="ICommandLineCommand"/>
        /// </summary>
        public IArgument Argument { get; private set; }

        /// <summary>
        /// Creates a new CLI Parser Exception for a given argument
        /// </summary>
        /// <param name="argument">The argument</param>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">Optional inner exception</param>
        public BaseParserException(IArgument argument, string message, Exception innerException = null)
            : base(message, innerException)
        {
            this.Argument = argument;
        }
    }
}
