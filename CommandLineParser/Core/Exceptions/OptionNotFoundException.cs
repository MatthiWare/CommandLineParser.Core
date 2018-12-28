using System.Collections.Generic;

using MatthiWare.CommandLine.Abstractions;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Indiciates the configured required option is not found
    /// </summary>
    public class OptionNotFoundException : KeyNotFoundException
    {
        /// <summary>
        /// Option that was not found
        /// </summary>
        public ICommandLineOption Option { get; private set; }

        public OptionNotFoundException(CommandLineParserOptions parserOptions, ICommandLineOption option)
            : base(CreateMessage(parserOptions, option))
        { }

        private static string CreateMessage(CommandLineParserOptions parserOptions, ICommandLineOption option)
        {
            bool hasShort = option.HasShortName;
            bool hasLong = option.HasLongName;
            bool hasBoth = hasShort && hasLong;

            string hasBothSeperator = hasBoth ? "|" : string.Empty;
            string shortName = hasShort ? $"{parserOptions.PrefixShortOption}{option.ShortName}" : string.Empty;
            string longName = hasLong ? $"{parserOptions.PrefixLongOption}{option.LongName}" : string.Empty;

            return $"Required argument {shortName}{hasBothSeperator}{longName} not found!";
        }
    }
}
