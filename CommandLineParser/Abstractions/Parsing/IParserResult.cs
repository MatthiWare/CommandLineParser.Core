using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    /// <summary>
    /// Parser result
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IParserResult<TResult>
    {
        /// <summary>
        /// Returns true if the user specified a help option
        /// </summary>
        bool HelpRequested { get; }

        /// <summary>
        /// Help was requested for this <see cref="ICommandLineOption"/> or <see cref="ICommandLineCommand"/>
        /// </summary>
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
        /// <param name="cancellationToken">cancellation token</param>
        /// /// <exception cref="InvalidOperationException">
        /// Result contains exceptions. For more info see <see cref="HasErrors"/> and <see cref="Errors"/> properties.
        /// </exception>
        Task ExecuteCommandsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Read-only collection that contains the parsed commands' results. 
        /// <see cref="ICommandParserResult"/>
        /// </summary>
        IReadOnlyList<ICommandParserResult> CommandResults { get; }
    }
}
