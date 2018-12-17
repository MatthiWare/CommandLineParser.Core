using System;
using System.Collections.Generic;

using MatthiWare.CommandLine.Abstractions.Parsing.Command;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    public interface IParserResult<TResult>
    {
        /// <summary>
        /// Parsed result
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Result contains exceptions. For more info see <see cref="HasErrors"/> and <see cref="Error"/> properties.
        /// </exception>
        TResult Result { get; }

        /// <summary>
        /// Returns true if any exceptions occured during parsing.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Contains the thrown exception during parsing.
        /// </summary>
        Exception Error { get; }

        /// <summary>
        /// Executes the commands
        /// </summary>
        /// /// <exception cref="InvalidOperationException">
        /// Result contains exceptions. For more info see <see cref="HasErrors"/> and <see cref="Error"/> properties.
        /// </exception>
        void ExecuteCommands();

        /// <summary>
        /// Read-only collection that contains the parsed commands' results. 
        /// <see cref="ICommandParserResult"/>
        /// </summary>
        IReadOnlyList<ICommandParserResult> CommandResults { get; }
    }
}
