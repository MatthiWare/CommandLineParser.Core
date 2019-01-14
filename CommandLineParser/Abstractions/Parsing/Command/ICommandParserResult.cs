using System;
using System.Collections;
using System.Collections.Generic;
using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Abstractions.Parsing.Command
{
    /// <summary>
    /// Results fo the command that has been parsed
    /// </summary>
    public interface ICommandParserResult
    {
        /// <summary>
        /// Indicates if the command has been found.
        /// </summary>
        bool Found { get; }

        /// <summary>
        /// Specifies the command/option that the help display has been requested for
        /// </summary>
        IArgument HelpRequestedFor { get; }

        /// <summary>
        /// Returns true if the user specified a help option
        /// </summary>
        bool HelpRequested { get; }

        /// <summary>
        /// Subcommands of the current command
        /// </summary>
        IReadOnlyCollection<ICommandParserResult> SubCommands { get; }

        /// <summary>
        /// The associated command
        /// </summary>
        ICommandLineCommand Command { get; }

        /// <summary>
        /// Returns true if any exceptions occured during parsing.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Contains the thrown exception(s) during parsing.
        /// </summary>
        IReadOnlyCollection<Exception> Errors { get; }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Result contains exceptions. For more info see <see cref="HasErrors"/> and <see cref="Errors"/> properties.
        /// </exception>
        void ExecuteCommand();
    }
}
