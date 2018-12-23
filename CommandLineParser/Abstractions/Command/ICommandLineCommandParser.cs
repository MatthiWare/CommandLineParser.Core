using System.Collections.Generic;

using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;

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
        /// <param name="argumentManager">Arguments to parse</param>
        /// <returns><see cref="ICommandParserResult"/></returns>
        ICommandParserResult Parse(IArgumentManager argumentManager);
    }
}
