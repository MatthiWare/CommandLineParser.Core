using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    /// <summary>
    /// Managers the arguments
    /// </summary>
    public interface IArgumentManager
    {
        /// <summary>
        /// Tries to get the arguments associated to the current command
        /// </summary>
        /// <param name="cmd">the command</param>
        /// <param name="model">The result arguments</param>
        /// <returns>True if arguments are found, false if not</returns>
        bool TryGetValue(ICommandLineCommand cmd, out ArgumentModel model);
    }
}
