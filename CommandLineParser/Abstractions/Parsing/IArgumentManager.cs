using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Usage;
using System;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    /// <summary>
    /// Managers the arguments
    /// </summary>
    public interface IArgumentManager
    {
        /// <summary>
        /// Returns a read-only list of arguments that never got processed because they appeared after the <see cref="CommandLineParserOptions.StopParsingAfter"/> flag.
        /// </summary>
        IReadOnlyList<UnusedArgumentModel> UnprocessedArguments { get; }

        /// <summary>
        /// Returns a read-only list of unused arguments. 
        /// In most cases this will be mistyped arguments that are not mapped to the actual option/command names.
        /// You can pass these arguments inside the <see cref="IUsagePrinter.PrintSuggestion(UnusedArgumentModel)"/> to get a suggestion of what could be the correct argument.
        /// </summary>
        IReadOnlyList<UnusedArgumentModel> UnusedArguments { get; }

        /// <summary>
        /// Returns if the <see cref="CommandLineParserOptions.StopParsingAfter"/> flag was found. 
        /// </summary>
        bool StopParsingFlagSpecified { get; }

        /// <summary>
        /// Tries to get the arguments associated to the current option
        /// </summary>
        /// <param name="argument">the argument</param>
        /// <param name="model">The result arguments</param>
        /// <returns>True if arguments are found, false if not</returns>
        bool TryGetValue(IArgument argument, out ArgumentModel model);

        /// <summary>
        /// Processes the argument list
        /// </summary>
        /// <param name="arguments">Input arguments</param>
        /// <param name="errors">List of processesing errors</param>
        /// <param name="commandContainer">Container for the commands and options</param>
        void Process(IReadOnlyList<string> arguments, IList<Exception> errors, ICommandLineCommandContainer commandContainer);
    }
}
