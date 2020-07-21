using MatthiWare.CommandLine.Abstractions.Command;
using System;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Command failed to execute exception
    /// </summary>
    public class CommandExecutionFailedException : Exception
    {
        /// <summary>
        /// Command failed to execute exception
        /// </summary>
        /// <param name="command">Command that failed</param>
        /// <param name="innerException">Actual exception</param>
        public CommandExecutionFailedException(ICommandLineCommand command, Exception innerException)
            : base(CreateMessage(command, innerException), innerException)
        {
        }

        private static string CreateMessage(ICommandLineCommand command, Exception exception)
            => $"Command '{command.Name}' failed to execute because: {exception.Message}";
    }
}
