using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace MatthiWare.CommandLine
{
    /// <summary>
    /// Command line parser
    /// </summary>
    public class CommandLineParser : CommandLineParser<object>
    {
        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        public CommandLineParser()
            : base()
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="parserOptions">The parser options</param>
        public CommandLineParser(CommandLineParserOptions parserOptions)
            : base(parserOptions)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="serviceCollection">Services collection to use, if null will create an internal one</param>
        public CommandLineParser(IServiceCollection serviceCollection)
            : base(new CommandLineParserOptions(), serviceCollection)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="parserOptions">options that the parser will use</param>
        /// <param name="serviceCollection">Services collection to use, if null will create an internal one</param>
        public CommandLineParser(CommandLineParserOptions parserOptions, IServiceCollection serviceCollection)
            : base(parserOptions, serviceCollection)
        { }
    }
}
