using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using System.Threading;
using System.Threading.Tasks;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Parser for a command line command
    /// </summary>
    public interface ICommandLineCommandParser
    {
        /// <summary>
        /// Parses the arguments
        /// </summary>
        /// <returns><see cref="ICommandParserResult"/></returns>
        Task<ICommandParserResult> ParseAsync(CancellationToken cancellationToken);
    }
}
