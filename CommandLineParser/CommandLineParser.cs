using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Parsing;

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
        /// <param name="argumentResolverFactory">argument resolver to use</param>
        public CommandLineParser(IArgumentResolverFactory argumentResolverFactory)
            : base(argumentResolverFactory)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="parserOptions">options that the parser will use</param>
        /// <param name="argumentResolverFactory">argument resolver to use</param>
        public CommandLineParser(CommandLineParserOptions parserOptions, IArgumentResolverFactory argumentResolverFactory)
            : base(parserOptions, argumentResolverFactory)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="containerResolver">container resolver to use</param>
        public CommandLineParser(IContainerResolver containerResolver)
            : base(new CommandLineParserOptions(), containerResolver)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="parserOptions">options that the parser will use</param>
        /// <param name="containerResolver">container resolver to use</param>
        public CommandLineParser(CommandLineParserOptions parserOptions, IContainerResolver containerResolver)
            : base(parserOptions, containerResolver)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="argumentResolverFactory">argument resolver to use</param>
        /// <param name="containerResolver">container resolver to use</param>
        public CommandLineParser(CommandLineParserOptions parserOptions, IArgumentResolverFactory argumentResolverFactory, IContainerResolver containerResolver)
            : base(parserOptions, argumentResolverFactory, containerResolver)
        { }
    }
}
