using System;
using System.Collections.Generic;
using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Unable to parse the command
    /// </summary>
    public class CommandParseException : Exception
    {
        /// <summary>
        /// Command that caused the parsing error
        /// </summary>
        public ICommandLineCommand Command { get; set; }

        public CommandParseException(ICommandLineCommand command, IReadOnlyCollection<Exception> innerExceptions)
            : base("", new AggregateException(innerExceptions))
        {
            Command = command;
        }
    }
}
