using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;

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
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use extension method 'AddCommandLineParser' to register the parser with DI")]
        public CommandLineParser(IServiceCollection serviceCollection)
            : this(new CommandLineParserOptions(), serviceCollection)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="parserOptions">options that the parser will use</param>
        /// <param name="serviceCollection">Services collection to use, if null will create an internal one</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use extension method 'AddCommandLineParser' to register the parser with DI")]
        public CommandLineParser(CommandLineParserOptions parserOptions, IServiceCollection serviceCollection)
            : base(parserOptions, serviceCollection)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="serviceProvider">Services provider to use</param>
        public CommandLineParser(IServiceProvider serviceProvider)
            : this(new CommandLineParserOptions(), serviceProvider)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="parserOptions">options that the parser will use</param>
        /// <param name="serviceProvider">Services Provider to use</param>
        public CommandLineParser(CommandLineParserOptions parserOptions, IServiceProvider serviceProvider)
            : base(parserOptions, serviceProvider)
        { }
    }
}
