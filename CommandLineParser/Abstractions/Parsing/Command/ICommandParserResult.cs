using System;

using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Abstractions.Parsing.Command
{
    /// <summary>
    /// Results fo the command that has been parsed
    /// </summary>
    public interface ICommandParserResult
    {
        ICommandLineCommand Command { get; }

        /// <summary>
        /// Returns true if any exceptions occured during parsing.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Contains the thrown exception during parsing.
        /// </summary>
        Exception Error { get; }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// /// <exception cref="InvalidOperationException">
        /// Result contains exceptions. For more info see <see cref="HasErrors"/> and <see cref="Error"/> properties.
        /// </exception>
        void ExecuteCommand();
    }
}
