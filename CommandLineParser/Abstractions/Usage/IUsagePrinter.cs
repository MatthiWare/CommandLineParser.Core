using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MatthiWare.CommandLine.Abstractions.Usage
{
    /// <summary>
    /// CLI Usage Output Printer
    /// </summary>
    public interface IUsagePrinter
    {
        /// <summary>
        /// Gets the usage builder <see cref="IUsageBuilder"/>
        /// </summary>
        IUsageBuilder Builder { get; }

        /// <summary>
        /// Print global usage
        /// </summary>
        void PrintUsage();

        /// <summary>
        /// Print an argument
        /// </summary>
        /// <param name="argument">The given argument</param>
        [Obsolete("Use PrintCommandUsage or PrintOptionUsage instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void PrintUsage(IArgument argument);

        /// <summary>
        /// Print command usage
        /// </summary>
        /// <param name="command">The given command</param>
        [Obsolete("Use PrintCommandUsage instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void PrintUsage(ICommandLineCommand command);

        /// <summary>
        /// Print command usage
        /// </summary>
        /// <param name="command">The given command</param>
        void PrintCommandUsage(ICommandLineCommand command);

        /// <summary>
        /// Print option usage
        /// </summary>
        /// <param name="option">The given option</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use PrintCommandUsage instead")]
        void PrintUsage(ICommandLineOption option);

        /// <summary>
        /// Print option usage
        /// </summary>
        /// <param name="option">The given option</param>
        void PrintOptionUsage(ICommandLineOption option);
        
        /// <summary>
        /// Print errors
        /// </summary>
        /// <param name="errors">list of errors</param>
        void PrintErrors(IReadOnlyCollection<Exception> errors);

        /// <summary>
        /// Prints suggestions based on the input arguments
        /// </summary>
        /// <param name="model">Input model</param>
        /// <returns>True if a suggestion was found and printed, otherwise false</returns>
        bool PrintSuggestion(UnusedArgumentModel model);
    }
}
