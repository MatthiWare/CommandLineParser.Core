using MatthiWare.CommandLine.Abstractions.Models;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    /// <summary>
    /// Managers the arguments
    /// </summary>
    public interface IArgumentManager
    {
        /// <summary>
        /// Returns a read-only list of unused arguments
        /// </summary>
        IReadOnlyList<UnusedArgumentModel> UnusedArguments { get; }

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
        void Process(IReadOnlyList<string> arguments);
    }
}
