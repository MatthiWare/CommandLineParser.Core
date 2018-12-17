using System.Collections.Generic;

using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Indicitates that a configured required command is not found.
    /// </summary>
    public class CommandNotFoundException : KeyNotFoundException
    {
        /// <summary>
        /// The command that was not found
        /// </summary>
        public ICommandLineCommand Command { get; private set; }

        public CommandNotFoundException(ICommandLineCommand cmd)
            : base($"Required command '{cmd.HasShortName}' or '{cmd.LongName}' not found!")
        {
            Command = cmd;
        }
    }
}
