using System;
using System.Collections.Generic;
using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Unable to parse the command
    /// </summary>
    public class CommandParseException : BaseParserException
    {
        /// <summary>
        /// Command that caused the parsing error
        /// </summary>
        public ICommandLineCommand Command => (ICommandLineCommand)Argument;

        /// <summary>
        /// Creates a new command parse exception
        /// </summary>
        /// <param name="command">the failed command</param>
        /// <param name="innerExceptions">collection of inner exception</param>
        public CommandParseException(ICommandLineCommand command, IReadOnlyCollection<Exception> innerExceptions)
            : base(command, "", new AggregateException(innerExceptions))
        { }
    }
}
