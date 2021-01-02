using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            : base(command, CreateMessage(command, innerExceptions), new AggregateException(innerExceptions))
        { }

        private static string CreateMessage(ICommandLineCommand command, IReadOnlyCollection<Exception> exceptions)
        {
            if (exceptions.Count > 1)
            {
                return CreateMultipleExceptionsMessage(command, exceptions);
            }
            else
            {
                return CreateSingleExceptionMessage(command, exceptions.First());
            }
        }

        private static string CreateSingleExceptionMessage(ICommandLineCommand command, Exception exception)
            => $"Unable to parse command '{command.Name}' because {exception.Message}";


        private static string CreateMultipleExceptionsMessage(ICommandLineCommand command, IReadOnlyCollection<Exception> exceptions)
        {
            var message = new StringBuilder();
            message.AppendLine($"Unable to parse command '{command.Name}' because {exceptions.Count} errors occured");

            foreach (var exception in exceptions)
            {
                message.AppendLine($" - {exception.Message}");
            }

            return message.ToString();
        }
    }
}
