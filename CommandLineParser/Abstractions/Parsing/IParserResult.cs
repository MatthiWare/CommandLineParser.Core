using System;
using System.Collections.Generic;

using MatthiWare.CommandLine.Abstractions.Parsing.Command;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    public interface IParserResult<TResult>
    {
        /// <summary>
        /// Returns true if the user specified a help option
        /// </summary>
        bool HelpRequested { get; }

        IArgument HelpRequestedFor { get; }

        /// <summary>
        /// Parsed result
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Result contains exceptions. For more info see <see cref="HasErrors"/> and <see cref="Errors"/> properties.
        /// </exception>
        TResult Result { get; }

        /// <summary>
        /// Returns true if any exceptions occured during parsing.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Contains the thrown exception during parsing.
        /// </summary>
        IReadOnlyCollection<Exception> Errors { get; }

        /// <summary>
        /// Executes the commands
        /// </summary>
        /// /// <exception cref="InvalidOperationException">
        /// Result contains exceptions. For more info see <see cref="HasErrors"/> and <see cref="Errors"/> properties.
        /// </exception>
        void ExecuteCommands();

        /// <summary>
        /// Read-only collection that contains the parsed commands' results. 
        /// <see cref="ICommandParserResult"/>
        /// </summary>
        IReadOnlyList<ICommandParserResult> CommandResults { get; }
    }
}
