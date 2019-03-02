using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Indicitates that a configured required command is not found.
    /// </summary>
    public class CommandNotFoundException : BaseParserException
    {
        /// <summary>
        /// The command that was not found
        /// </summary>
        public ICommandLineCommand Command => (ICommandLineCommand)Argument;

        /// <summary>
        /// Creates a new command not found exception
        /// </summary>
        /// <param name="cmd">The command that was not found</param>
        public CommandNotFoundException(ICommandLineCommand cmd)
            : base(cmd, $"Required command '{cmd.Name}' not found!")
        { }
    }
}
